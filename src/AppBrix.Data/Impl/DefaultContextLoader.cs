// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Data.Migrations;
using AppBrix.Lifecycle;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Loader;

namespace AppBrix.Data.Impl
{
    internal class DefaultContextLoader : IContextLoader, IApplicationLifecycle
    {
        #region Properties
        public IApp App { get; private set; }
        #endregion

        #region Public and overriden methods
        public void Initialize(IInitializeContext context)
        {
            this.App = context.App;
        }

        public void Uninitialize()
        {
            this.App = null;
        }

        public T Get<T>() where T : DbContextBase
        {
            T context;
            
            var type = typeof(T);
            if (!this.InitializedContexts.Contains(type))
            {
                this.InitializedContexts.Add(type);
                this.MigrateMigrationsContext();
                this.MigrateContext(type);
            }

            context = this.App.GetFactory().Get<T>();
            context.Initialize(new DefaultInitializeDbContext(this.App, null));
            return context;
        }
        #endregion

        #region Private methods
        private void MigrateMigrationsContext()
        {
            var migrationsContextType = typeof(MigrationsContext);
            if (!this.InitializedContexts.Contains(migrationsContextType))
            {
                this.InitializedContexts.Add(migrationsContextType);
                using (var context = (DbContext)this.App.GetFactory().Get(migrationsContextType))
                {
                    context.Database.Migrate();
                }
            }
        }

        private void MigrateContext(Type type)
        {
            SnapshotData snapshot;
            var assemblyVersion = type.GetTypeInfo().Assembly.GetName().Version;
            using (var context = this.App.GetFactory().Get<MigrationsContext>())
            {
                snapshot = context.Snapshots.FirstOrDefault(x => x.Context == type.Name);
            }

            if (snapshot == null || Version.Parse(snapshot.Version) < assemblyVersion)
            {
                var oldVersion = Version.Parse(snapshot?.Version ?? DefaultContextLoader.EmptyVersion);
                var oldMigrationsAssembly = this.GenerateMigrationAssemblyName(type, oldVersion);
                this.LoadAssembly(oldMigrationsAssembly, type, snapshot?.Snapshot ?? string.Empty);

                var newVersion = type.GetTypeInfo().Assembly.GetName().Version;
                var newMigrationName = this.GenerateMigrationName(type, newVersion);
                var newMigrationsAssembly = this.GenerateMigrationAssemblyName(type, newVersion);
                ScaffoldedMigration scaffoldedMigration;
                using (var context = (DbContextBase)this.App.GetFactory().Get(type))
                {
                    context.Initialize(new DefaultInitializeDbContext(this.App, oldMigrationsAssembly));
                    scaffoldedMigration = this.CreateMigration(context, newMigrationName);
                }

                var migration = new MigrationData()
                {
                    Context = type.Name,
                    Version = newVersion.ToString(),
                    Migration = scaffoldedMigration.MigrationCode,
                    Metadata = scaffoldedMigration.MetadataCode
                };
                this.LoadAssembly(newMigrationsAssembly, type, scaffoldedMigration.SnapshotCode, migration);
                using (var context = (DbContextBase)this.App.GetFactory().Get(type))
                {
                    context.Initialize(new DefaultInitializeDbContext(this.App, newMigrationsAssembly));
                    context.Database.Migrate();
                }
                
                this.UpdateSnapshot(type.Name, newVersion.ToString(), scaffoldedMigration.SnapshotCode, snapshot == null, migration);
            }
        }

        private void LoadAssembly(string assemblyName, Type type, string snapshot, params MigrationData[] migrations)
        {
            var parseOptions =
                CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp6)
                    .WithDocumentationMode(DocumentationMode.None);
            var compilation = CSharpCompilation.Create(assemblyName,
                syntaxTrees: this.GetSyntaxTrees(snapshot, migrations),
                references: this.GetReferences(type),
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);
                ms.Seek(0, SeekOrigin.Begin);
                AssemblyLoadContext.Default.LoadFromStream(ms);
            }
        }
        
        private ICollection<SyntaxTree> GetSyntaxTrees(string snapshot, params MigrationData[] migrations)
        {
            var trees = new List<SyntaxTree>();

            if (snapshot != null)
            {
                trees.Add(SyntaxFactory.ParseSyntaxTree(snapshot));
            }

            if (migrations != null)
            {
                foreach (var migration in migrations)
                {
                    trees.Add(SyntaxFactory.ParseSyntaxTree(migration.Migration));
                    trees.Add(SyntaxFactory.ParseSyntaxTree(migration.Metadata));
                }
            }

            return trees;
        }

        private IEnumerable<MetadataReference> GetReferences(Type type)
        {
            var assemblies = new HashSet<string>();
            this.GetReferences(Assembly.GetEntryAssembly(), assemblies);
            return assemblies.Select(x => MetadataReference.CreateFromFile(x));
        }

        private void GetReferences(Assembly assembly, HashSet<string> locations, HashSet<string> names = null)
        {
            if (names == null)
                names = new HashSet<string>();

            if (!names.Contains(assembly.FullName))
            {
                names.Add(assembly.FullName);
                locations.Add(assembly.Location);
            }

            var newAssemblies = new List<AssemblyName>();
            foreach (var referencedAssemblyName in assembly.GetReferencedAssemblies())
            {
                if (!names.Contains(referencedAssemblyName.FullName))
                {
                    names.Add(referencedAssemblyName.FullName);
                    newAssemblies.Add(referencedAssemblyName);
                }
            }

            foreach (var referencedAssemblyName in newAssemblies)
            {
                var referencedAssembly = Assembly.Load(referencedAssemblyName);
                if (locations.Contains(referencedAssembly.Location))
                    continue;

                locations.Add(referencedAssembly.Location);
                GetReferences(referencedAssembly, locations, names);
            }
        }

        private ScaffoldedMigration CreateMigration(DbContextBase context, string migrationName)
        {
            var codeHelper = new CSharpHelper();
            var scaffolder = ActivatorUtilities.CreateInstance<MigrationsScaffolder>(
                ((IInfrastructure<IServiceProvider>)context).Instance,
                new CSharpMigrationsGenerator(
                    codeHelper,
                    new CSharpMigrationOperationGenerator(codeHelper),
                    new CSharpSnapshotGenerator(codeHelper)));

            return scaffolder.ScaffoldMigration(migrationName, context.GetType().Namespace);
        }

        private void UpdateSnapshot(string contextName, string version, string snapshot, bool createNew, MigrationData migration)
        {
            using (var context = this.App.GetFactory().Get<MigrationsContext>())
            {
                SnapshotData newSnapshot;
                if (createNew)
                {
                    newSnapshot = new SnapshotData() { Context = contextName };
                    context.Snapshots.Add(newSnapshot);
                }
                else
                {
                    newSnapshot = context.Snapshots.First(x => x.Context == contextName);
                }
                newSnapshot.Version = version;
                newSnapshot.Snapshot = snapshot;
                context.Migrations.Add(migration);
                context.SaveChanges();
            }
        }

        private string GenerateMigrationAssemblyName(Type type, Version version = null)
        {
            if (version == null)
                version = type.GetTypeInfo().Assembly.GetName().Version;

            return string.Format("Generated.Migrations.{0}.{1}.dll",
                this.GenerateMigrationName(type, version), Guid.NewGuid());
        }

        private string GenerateMigrationName(Type type, Version version)
        {
            return string.Format("{0}_{1}", type.Name, version.ToString().Replace('.', '_'));
        }
        #endregion

        #region Private fields and constants
        private const string EmptyVersion = "0.0.0.0";
        private readonly HashSet<Type> InitializedContexts = new HashSet<Type>();
        #endregion
    }
}
