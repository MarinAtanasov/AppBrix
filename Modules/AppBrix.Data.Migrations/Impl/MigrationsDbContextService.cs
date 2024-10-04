// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Data.Data;
using AppBrix.Data.Migrations.Configuration;
using AppBrix.Data.Migrations.Data;
using AppBrix.Data.Migrations.Events;
using AppBrix.Data.Services;
using AppBrix.Lifecycle;
using AppBrix.Logging.Configuration;
using AppBrix.Logging.Contracts;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;

namespace AppBrix.Data.Migrations.Impl;

internal sealed class MigrationsDbContextService : IDbContextService, IApplicationLifecycle
{
    #region Public and overriden methods
    public void Initialize(IInitializeContext context)
    {
        this.app = context.App;
        this.config = this.app.ConfigService.GetMigrationsDataConfig();
        this.contextService = this.app.GetDbContextService();
        this.loggingConfig = this.app.ConfigService.GetLoggingConfig();
    }

    public void Uninitialize()
    {
        this.app = null!;
        this.config = null!;
        this.contextService = null!;
        this.loggingConfig = null!;
        this.initializedContexts.Clear();
    }

    public DbContext Get(Type type)
    {
        if (type is null)
            throw new ArgumentNullException(nameof(type));

        this.MigrateContextIfNeeded(type);

        return this.contextService.Get(type);
    }
    #endregion

    #region Private methods
    private SnapshotData? GetSnapshot(Type type)
    {
        SnapshotData? snapshot = null;
        var logLevel = LogLevel.None;

        try
        {
            using var context = this.contextService.GetMigrationsContext();
            if (type == typeof(MigrationsDbContext))
            {
                logLevel = this.loggingConfig.LogLevel;
                this.loggingConfig.LogLevel = LogLevel.Critical;
            }
            snapshot = context.Snapshots.AsNoTracking().SingleOrDefault(x => x.Context == type.Name);
        }
        catch (Exception) { }
        finally
        {
            if (type == typeof(MigrationsDbContext))
                this.loggingConfig.LogLevel = logLevel;
        }

        return snapshot;
    }

    private void MigrateContextIfNeeded(Type type)
    {
        if (!this.initializedContexts.Contains(type))
        {
            lock (this.initializedContexts)
            {
                if (!typeof(DbContextBase).IsAssignableFrom(type))
                {
                    this.initializedContexts.Add(type);
                    return;
                }

                this.MigrateContext(typeof(MigrationsDbContext));
                this.MigrateContext(type);
            }
        }
    }

    private void MigrateContext(Type type)
    {
        if (this.initializedContexts.Contains(type))
            return;

        var snapshot = this.GetSnapshot(type);
        var assemblyVersion = type.Assembly.GetName().Version;
        if (assemblyVersion is null || 
            snapshot is not null && Version.Parse(snapshot.Version) >= assemblyVersion)
        {
            this.initializedContexts.Add(type);
            return;
        }

        var oldSnapshotCode = snapshot?.Snapshot ?? string.Empty;
        var oldVersion = snapshot is null ? MigrationsDbContextService.EmptyVersion : Version.Parse(snapshot.Version);
        var oldMigrationsAssembly = this.GenerateMigrationAssemblyName(type, oldVersion);
        this.LoadAssembly(type, oldMigrationsAssembly, oldSnapshotCode);
        var newMigrationName = this.GenerateMigrationName(type, assemblyVersion);

        try
        {
            var scaffoldedMigration = this.CreateMigration(type, oldMigrationsAssembly, newMigrationName);
            var migration = scaffoldedMigration.SnapshotCode != oldSnapshotCode ?
                this.ApplyMigration(type, assemblyVersion, scaffoldedMigration) : null;
            this.AddMigration(type.Name, assemblyVersion.ToString(), migration, scaffoldedMigration.SnapshotCode, snapshot);
        }
        catch (InvalidOperationException)
        {
            // Context does not support migrations.
            using var context = this.contextService.Get(type);
            context.Database.EnsureCreated();
        }

        this.initializedContexts.Add(type);
        this.app.GetEventHub().Raise(new DbContextMigratedEvent(oldVersion, assemblyVersion, type));
    }

    private void LoadAssembly(Type type, string assemblyName, string snapshot, params MigrationData[] migrations)
    {
        var compilation = CSharpCompilation.Create(assemblyName,
            syntaxTrees: this.GetSyntaxTrees(snapshot, migrations),
            references: this.GetReferences(type),
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var ms = new MemoryStream();
        compilation.Emit(ms);
        ms.Seek(0, SeekOrigin.Begin);
        AssemblyLoadContext.Default.LoadFromStream(ms);
    }

    private IEnumerable<SyntaxTree> GetSyntaxTrees(string snapshot, params MigrationData[] migrations)
    {
        var trees = new List<SyntaxTree>();

        if (snapshot is not null)
        {
            trees.Add(SyntaxFactory.ParseSyntaxTree(snapshot));
        }

        if (migrations is not null)
        {
            foreach (var migration in migrations)
            {
                trees.Add(SyntaxFactory.ParseSyntaxTree(migration.Migration));
                trees.Add(SyntaxFactory.ParseSyntaxTree(migration.Metadata));
            }
        }

        return trees;
    }

    private IEnumerable<MetadataReference> GetReferences(Type type) => type.Assembly
        .GetReferencedAssemblies(recursive: true)
        .Select(x => MetadataReference.CreateFromFile(x.Location));

    private ScaffoldedMigration CreateMigration(Type type, string oldMigrationsAssembly, string migrationName)
    {
        using var context = (DbContextBase)this.contextService.Get(type);
        context.Initialize(new InitializeDbContext(this.app, oldMigrationsAssembly, this.GenerateMigrationAssemblyName(type)));
        var scaffolder = this.CreateMigrationsScaffolder(context);
        return scaffolder.ScaffoldMigration(migrationName, context.GetType().Namespace);
    }

    private MigrationsScaffolder CreateMigrationsScaffolder(DbContext context)
    {
        var logger = this.app.GetLogHub();

#pragma warning disable EF1001 // Internal EF Core API usage.
        var reporter = new OperationReporter(new OperationReportHandler(
            m => logger.Error(m),
            m => logger.Warning(m),
            m => logger.Info(m),
            m => logger.Trace(m)));

        var designTimeServices = new ServiceCollection()
            .AddSingleton(context.Model)
            .AddSingleton(context.GetService<IConventionSetBuilder>())
            .AddSingleton(context.GetService<ICurrentDbContext>())
            .AddSingleton(context.GetService<IDatabaseProvider>())
            .AddSingleton(context.GetService<IDesignTimeModel>().Model)
            .AddSingleton(context.GetService<IHistoryRepository>())
            .AddSingleton(context.GetService<IMigrationsAssembly>())
            .AddSingleton(context.GetService<IMigrationsIdGenerator>())
            .AddSingleton(context.GetService<IMigrationsModelDiffer>())
            .AddSingleton(context.GetService<IMigrator>())
            .AddSingleton(context.GetService<IModelRuntimeInitializer>())
            .AddSingleton(context.GetService<IRelationalTypeMappingSource>())
            .AddSingleton(context.GetService<ITypeMappingSource>())
            .AddSingleton<IAnnotationCodeGenerator, AnnotationCodeGenerator>()
            .AddSingleton<ICSharpHelper, CSharpHelper>()
            .AddSingleton<ICSharpMigrationOperationGenerator, CSharpMigrationOperationGenerator>()
            .AddSingleton<ICSharpSnapshotGenerator, CSharpSnapshotGenerator>()
            .AddSingleton<IMigrationsCodeGenerator, CSharpMigrationsGenerator>()
            .AddSingleton<IMigrationsCodeGeneratorSelector, MigrationsCodeGeneratorSelector>()
            .AddSingleton<IOperationReporter>(reporter)
            .AddSingleton<ISnapshotModelProcessor, SnapshotModelProcessor>()
            .AddSingleton<AnnotationCodeGeneratorDependencies>()
            .AddSingleton<CSharpMigrationOperationGeneratorDependencies>()
            .AddSingleton<CSharpMigrationsGeneratorDependencies>()
            .AddSingleton<CSharpSnapshotGeneratorDependencies>()
            .AddSingleton<MigrationsCodeGeneratorDependencies>()
            .AddSingleton<MigrationsScaffolderDependencies>()
            .AddSingleton<MigrationsScaffolder>()
            .BuildServiceProvider();
#pragma warning restore EF1001 // Internal EF Core API usage.

        return designTimeServices.GetRequiredService<MigrationsScaffolder>();
    }

    private MigrationData ApplyMigration(Type type, Version version, ScaffoldedMigration scaffoldedMigration)
    {
        var migration = new MigrationData
        {
            Context = type.Name,
            Version = version.ToString(),
            Migration = scaffoldedMigration.MigrationCode,
            Metadata = scaffoldedMigration.MetadataCode
        };

        var migrationAssemblyName = this.GenerateMigrationAssemblyName(type, version);
        this.LoadAssembly(type, migrationAssemblyName, scaffoldedMigration.SnapshotCode, migration);
        using var context = (DbContextBase)this.contextService.Get(type);
        context.Initialize(new InitializeDbContext(this.app, migrationAssemblyName, this.GenerateMigrationsHistoryTableName(type)));
        context.Database.Migrate();
        return migration;
    }

    private void AddMigration(string contextName, string version, MigrationData? migration, string snapshotCode, SnapshotData? snapshot)
    {
        using var context = this.contextService.GetMigrationsContext();
        if (snapshot is null)
        {
            snapshot = new SnapshotData { Context = contextName };
            context.Snapshots.Add(snapshot);
        }
        else
        {
            context.Snapshots.Attach(snapshot);
        }

        snapshot.Version = version;
        if (migration is not null)
        {
            snapshot.Snapshot = snapshotCode;
            context.Migrations.Add(migration);
        }

        context.SaveChanges();
    }

    private string GenerateMigrationAssemblyName(Type type, Version? version = null) =>
        $"{typeof(MigrationsDataModule).Namespace}.Generated.{this.GenerateMigrationName(type, version ?? type.Assembly.GetName().Version!)}.{Guid.NewGuid()}.dll";

    private string GenerateMigrationName(Type type, Version version) => $"{type.Name}_{version.ToString().Replace('.', '_')}";

    private string GenerateMigrationsHistoryTableName(Type type)
    {
        var name = type.Name;
        foreach (var suffix in this.config.MigrationsHistoryTableSuffixes)
        {
            if (name.Length > suffix.Length && name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            {
                name = name[..^suffix.Length];
                break;
            }
        }

        return $"{this.config.MigrationsHistoryTablePrefix}{name}";
    }
    #endregion

    #region Private fields and constants
    private static readonly Version EmptyVersion = new Version();
    private readonly HashSet<Type> initializedContexts = [];
    private IApp app = null!;
    private MigrationsDataConfig config = null!;
    private IDbContextService contextService = null!;
    private LoggingConfig loggingConfig = null!;
    #endregion
}
