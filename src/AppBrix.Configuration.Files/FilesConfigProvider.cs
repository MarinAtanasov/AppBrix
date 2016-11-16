// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.IO;
using System.Linq;

namespace AppBrix.Configuration.Files
{
    /// <summary>
    /// An implementation of a file provider which stores each config inside its own file.
    /// </summary>
    public sealed class FilesConfigProvider : IConfigProvider
    {
        #region Construction
        /// <summary>
        /// Creates a new instance of <see cref="FilesConfigProvider"/>.
        /// </summary>
        /// <param name="directory">The directory where the configuration files are stored.</param>
        /// <param name="fileExtension">The extension to be used when reading/writing config files.</param>
        public FilesConfigProvider(string directory, string fileExtension = FilesConfigProvider.DefaultExtension)
        {
            if (string.IsNullOrEmpty(directory))
                throw new ArgumentNullException(nameof(directory));
            if (string.IsNullOrEmpty(fileExtension))
                throw new ArgumentNullException(nameof(fileExtension));

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            this.directory = directory;
            this.extension = fileExtension.TrimStart(FilesConfigProvider.ExtensionDot);
        }
        #endregion

        #region Public and overriden methods
        public string ReadConfig(Type type)
        {
            var path = this.BuildFilePath(type);
            return File.Exists(path) ? File.ReadAllText(path) : null;
        }

        public void WriteConfig(string config, Type type)
        {
            File.WriteAllText(this.BuildFilePath(type), config);
        }
        #endregion

        #region Private methods
        private string BuildFilePath(Type type)
        {
            var typeName = type.Name;
            var fileName = typeName.EndsWith(FilesConfigProvider.ToRemove, StringComparison.OrdinalIgnoreCase) && typeName.Length > ToRemove.Length ?
                typeName.Substring(0, typeName.Length - ToRemove.Length) : typeName;
            return Path.Combine(this.directory, string.Concat(fileName, FilesConfigProvider.ExtensionDot, this.extension));
        }
        #endregion

        #region Private fields and constants
        private const string DefaultExtension = "config";
        private const string ToRemove = "config";
        private const char ExtensionDot = '.';
        private readonly string directory;
        private readonly string extension;
        #endregion
    }
}
