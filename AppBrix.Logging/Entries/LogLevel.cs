// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Logging.Entries
{
    /// <summary>
    /// Used for selecting what log level is the current log entry.
    /// </summary>
    public enum LogLevel : int
    {
        All = 0,
        Trace = 1,
        Debug = 2,
        Info = 3,
        Warning = 4,
        Error = 5,
        Critical = 6,
        None = int.MaxValue
    }
}
