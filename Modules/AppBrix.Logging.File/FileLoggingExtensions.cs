// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Configuration;
using AppBrix.Logging.File.Configuration;

namespace AppBrix
{
    /// <summary>
    /// Extension methods for easier manipulation of AppBrix logging to a file.
    /// </summary>
    public static class FileLoggingExtensions
    {
        /// <summary>
        /// Gets the <see cref="FileLoggerConfig"/> from <see cref="IConfigService"/>.
        /// </summary>
        /// <param name="service">The configuration service.</param>
        /// <returns>The <see cref="FileLoggerConfig"/>.</returns>
        public static FileLoggerConfig GetFileLoggerConfig(this IConfigService service) => (FileLoggerConfig)service.Get(typeof(FileLoggerConfig));
    }
}
