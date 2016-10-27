// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Logging.Entries
{
    /// <summary>
    /// Used for selecting the severity level of the log entry.
    /// </summary>
    public enum LogLevel
    {
        Trace = 1,
        Debug = 2,
        Info = 3,
        Warning = 4,
        Error = 5,
        Critical = 6
    }
}
