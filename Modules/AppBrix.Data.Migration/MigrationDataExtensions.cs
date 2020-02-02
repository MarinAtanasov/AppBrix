// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Configuration;
using AppBrix.Data.Migration;
using AppBrix.Data.Migration.Configuration;

namespace AppBrix
{
    /// <summary>
    /// Extension methods for the <see cref="MigrationDataModule"/>.
    /// </summary>
    public static class MigrationDataExtensions
    {
        /// <summary>
        /// Gets the <see cref="MigrationDataConfig"/> from <see cref="IConfigService"/>.
        /// </summary>
        /// <param name="service">The configuration service.</param>
        /// <returns>The <see cref="MigrationDataConfig"/>.</returns>
        public static MigrationDataConfig GetMigrationDataConfig(this IConfigService service) => (MigrationDataConfig) service.Get(typeof(MigrationDataConfig));
    }
}
