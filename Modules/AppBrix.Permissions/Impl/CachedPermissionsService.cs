// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using System;
using System.Collections.Generic;

namespace AppBrix.Permissions.Impl
{
    internal sealed class CachedPermissionsService : IPermissionsService, IApplicationLifecycle
    {
        #region Construction
        public CachedPermissionsService(PermissionsService service)
        {
            this.service = service;
        }
        #endregion

        #region IApplicationLifecycle implementation
        public void Initialize(IInitializeContext context)
        {
            this.service.Initialize(context);
        }

        public void Uninitialize()
        {
            this.service.Uninitialize();
            this.cachedPermissions.Clear();
        }
        #endregion

        #region IPermissionsService implementation
        public void AddParent(string role, string parent)
        {
            this.service.AddParent(role, parent);
            this.CacheRolePermissions(role);
        }

        public void RemoveParent(string role, string parent)
        {
            this.service.RemoveParent(role, parent);
            this.CacheRolePermissions(role);
        }

        public IReadOnlyCollection<string> GetParents(string role) => this.service.GetParents(role);

        public IReadOnlyCollection<string> GetChildren(string role) => this.service.GetChildren(role);

        public void Allow(string role, string permission)
        {
            this.service.Allow(role, permission);
            this.CacheRolePermissions(role);
        }

        public void Deny(string role, string permission)
        {
            this.service.Deny(role, permission);
            this.CacheRolePermissions(role);
        }

        public void Unset(string role, string permission)
        {
            this.service.Unset(role, permission);
            this.CacheRolePermissions(role);
        }

        public bool HasPermission(string role, string permission)
        {
            if (string.IsNullOrEmpty(role))
                throw new ArgumentNullException(nameof(role));
            if (string.IsNullOrEmpty(permission))
                throw new ArgumentNullException(nameof(permission));

            return this.cachedPermissions.GetOrEmpty(role).Contains(permission);
        }

        public IReadOnlyCollection<string> GetAllowed(string role) => this.service.GetAllowed(role);

        public IReadOnlyCollection<string> GetDenied(string role) => this.service.GetDenied(role);
        #endregion

        #region Private methods
        private void CacheRolePermissions(string role)
        {
            var permissions = new HashSet<string>(this.service.GetAllowed(role));
            foreach (var parent in this.service.GetParents(role))
            {
                permissions.UnionWith(this.cachedPermissions.GetOrEmpty(parent));
            }
            permissions.ExceptWith(this.service.GetDenied(role));

            if (permissions.Count > 0)
            {
                this.cachedPermissions[role] = permissions;
            }
            else
            {
                this.cachedPermissions.Remove(role);
            }

            foreach (var child in this.service.GetChildren(role))
            {
                this.CacheRolePermissions(child);
            }
        }
        #endregion

        #region Private fields and constants
        private readonly Dictionary<string, HashSet<string>> cachedPermissions = new Dictionary<string, HashSet<string>>();
        private readonly PermissionsService service;
        #endregion
    }
}
