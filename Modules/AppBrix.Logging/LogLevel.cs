// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//

namespace AppBrix.Logging.Entries
{
    /// <summary>
    /// Used for selecting the severity level of the log entry.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Tracing logging severity.
        /// </summary>
        Trace = 1,
        /// <summary>
        /// Debug logging severity.
        /// </summary>
        Debug = 2,
        /// <summary>
        /// Information logging severity.
        /// </summary>
        Info = 3,
        /// <summary>
        /// Warning logging severity.
        /// </summary>
        Warning = 4,
        /// <summary>
        /// Error logging severity.
        /// </summary>
        Error = 5,
        /// <summary>
        /// Critical logging severity.
        /// </summary>
        Critical = 6
    }
}
