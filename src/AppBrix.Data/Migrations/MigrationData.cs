// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Data.Migrations
{
    public class MigrationData
    {
        public string Context { get; set; }
        public string Version { get; set; }

        public string Migration { get; set; }
        public string Metadata { get; set; }
    }
}
