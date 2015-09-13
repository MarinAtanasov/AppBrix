// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Utils.Exceptions
{
    public class DefaultConstructorNotFoundException : Exception
    {
        // Summary:
        //     Initializes a new instance of the AppBrix.DefaultConstructorMissingException class.
        public DefaultConstructorNotFoundException()
            : base()
        {
        }

        //
        // Summary:
        //     Initializes a new instance of the AppBrix.DefaultConstructorMissingException class with a specified
        //     error message.
        //
        // Parameters:
        //   message:
        //     The message that describes the error.
        public DefaultConstructorNotFoundException(string message)
            : base(message)
        {
        }
        
        //
        // Summary:
        //     Initializes a new instance of the AppBrix.DefaultConstructorMissingException class with a specified
        //     error message and a reference to the inner exception that is the cause of
        //     this exception.
        //
        // Parameters:
        //   message:
        //     The error message that explains the reason for the exception.
        //
        //   innerException:
        //     The exception that is the cause of the current exception, or a null reference
        //     (Nothing in Visual Basic) if no inner exception is specified.
        public DefaultConstructorNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
