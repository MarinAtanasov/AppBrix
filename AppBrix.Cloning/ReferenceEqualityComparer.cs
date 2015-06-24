// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBrix.Cloning
{
    internal class ReferenceEqualityComparer : EqualityComparer<object>
    {
        #region Public and overriden methods
        public override bool Equals(object x, object y)
        {
            return object.ReferenceEquals(x, y);
        }

        public override int GetHashCode(object obj)
        {
            return obj == null ? 0 : obj.GetHashCode();
        }
        #endregion
    }
}
