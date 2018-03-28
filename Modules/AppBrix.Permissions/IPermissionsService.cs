// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Collections.Generic;

namespace AppBrix.Permissions
{
    public interface IPermissionsService
    {
        void AddParent(string role, string parent);

        void RemoveParent(string role, string parent);

        IReadOnlyCollection<string> GetParents(string role);

        void Allow(string role, string permission);

        void Deny(string role, string permission);

        void Unset(string role, string permission);

        bool HasPermission(string role, string permission);
    }
}
