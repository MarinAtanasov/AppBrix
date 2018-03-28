// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBrix.Permissions.Impl
{
    internal sealed class DefaultPermissionsService : IPermissionsService, IApplicationLifecycle
    {
        #region IApplicationLifecycle implementation
        public void Initialize(IInitializeContext context)
        {
        }

        public void Uninitialize()
        {
            this.parents.Clear();
            this.allowed.Clear();
            this.denied.Clear();
        }
        #endregion

        #region IPermissionsService implementation
        public void AddParent(string role, string parent)
        {
            if (string.IsNullOrEmpty(role))
                throw new ArgumentNullException(nameof(role));
            if (string.IsNullOrEmpty(parent))
                throw new ArgumentNullException(nameof(parent));
            if (role == parent)
                throw new InvalidOperationException($"Trying to create circular dependency between {role} and {parent}.");
            if (this.GetAllParents(parent).Contains(role))
                throw new InvalidOperationException($"Trying to create circular dependency between {role} and {parent}.");

            this.parents.AddValue(role, parent);
        }

        public void RemoveParent(string role, string parent)
        {
            if (string.IsNullOrEmpty(role))
                throw new ArgumentNullException(nameof(role));
            if (string.IsNullOrEmpty(parent))
                throw new ArgumentNullException(nameof(parent));

            this.parents.RemoveValue(role, parent);
        }

        public IReadOnlyCollection<string> GetParents(string role)
        {
            if (string.IsNullOrEmpty(role))
                throw new ArgumentNullException(nameof(role));

            return this.parents.GetOrEmpty(role);
        }

        public void Allow(string role, string permission)
        {
            if (string.IsNullOrEmpty(role))
                throw new ArgumentNullException(nameof(role));
            if (string.IsNullOrEmpty(permission))
                throw new ArgumentNullException(nameof(permission));

            this.denied.RemoveValue(role, permission);
            this.allowed.AddValue(role, permission);
        }

        public void Deny(string role, string permission)
        {
            if (string.IsNullOrEmpty(role))
                throw new ArgumentNullException(nameof(role));
            if (string.IsNullOrEmpty(permission))
                throw new ArgumentNullException(nameof(permission));

            this.allowed.RemoveValue(role, permission);
            this.denied.AddValue(role, permission);
        }

        public void Unset(string role, string permission)
        {
            if (string.IsNullOrEmpty(role))
                throw new ArgumentNullException(nameof(role));
            if (string.IsNullOrEmpty(permission))
                throw new ArgumentNullException(nameof(permission));

            this.allowed.RemoveValue(role, permission);
            this.denied.RemoveValue(role, permission);
        }

        public bool HasPermission(string role, string permission)
        {
            if (string.IsNullOrEmpty(role))
                throw new ArgumentNullException(nameof(role));
            if (string.IsNullOrEmpty(permission))
                throw new ArgumentNullException(nameof(permission));

            return HasPermissionInternal(role, permission);
        }

        public IReadOnlyCollection<string> GetAllowed(string role)
        {
            if (string.IsNullOrEmpty(role))
                throw new ArgumentNullException(nameof(role));

            return this.allowed.GetOrEmpty(role);
        }

        public IReadOnlyCollection<string> GetDenied(string role)
        {
            if (string.IsNullOrEmpty(role))
                throw new ArgumentNullException(nameof(role));

            return this.denied.GetOrEmpty(role);
        }
        #endregion

        #region Private methods
        public IEnumerable<string> GetAllParents(string role)
        {
            if (this.parents.TryGetValue(role, out var roleParents))
            {
                return roleParents.Concat(roleParents.SelectMany(p => this.GetAllParents(p)));
            }
            else
            {
                return Array.Empty<string>();
            }
        }

        public bool HasPermissionInternal(string role, string permission)
        {
            if (this.denied.TryGetValue(role, out var roleDenied) && roleDenied.Contains(permission))
            {
                return false;
            }
            else if (this.allowed.TryGetValue(role, out var roleAllowed) && roleAllowed.Contains(permission))
            {
                return true;
            }
            else
            {
                return this.parents.TryGetValue(role, out var roleParents) && roleParents.Any(r => this.HasPermissionInternal(r, permission));
            }
        }
        #endregion

        #region Private fields and constants
        private Dictionary<string, HashSet<string>> parents = new Dictionary<string, HashSet<string>>();
        private Dictionary<string, HashSet<string>> allowed = new Dictionary<string, HashSet<string>>();
        private Dictionary<string, HashSet<string>> denied = new Dictionary<string, HashSet<string>>();
        #endregion
    }
}
