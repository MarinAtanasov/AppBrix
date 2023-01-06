// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Lifecycle;
using AppBrix.Permissions.Services;
using System;
using System.Collections.Generic;

namespace AppBrix.Permissions.Impl;

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
        this.cachedAllowed.Clear();
        this.cachedDenied.Clear();
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

        return this.cachedAllowed.TryGetValue(role, out var permissions) && permissions.Contains(permission);
    }

    public IReadOnlyCollection<string> GetAllowed(string role) => this.service.GetAllowed(role);

    public IReadOnlyCollection<string> GetDenied(string role) => this.service.GetDenied(role);
    #endregion

    #region Private methods
    /// <summary>
    /// Caches the permissions for the role and its children.
    /// Role Denied > Role Allowed > Parent Denied > Parent Allowed.
    /// </summary>
    /// <param name="role">The role to cache.</param>
    private void CacheRolePermissions(string role)
    {
        var parentsAllowed = new HashSet<string>();
        var parentsDenied = new HashSet<string>();
        foreach (var parent in this.service.GetParents(role))
        {
            if (this.cachedAllowed.TryGetValue(parent, out var parentAllowed) && parentAllowed.Count > 0)
                parentsAllowed.UnionWith(parentAllowed);
            if (this.cachedDenied.TryGetValue(parent, out var parentDenied) && parentDenied.Count > 0)
                parentsDenied.UnionWith(parentDenied);
        }

        var allowed = new HashSet<string>(this.service.GetAllowed(role));
        var denied = new HashSet<string>(this.service.GetDenied(role));

        parentsAllowed.ExceptWith(parentsDenied);
        parentsDenied.ExceptWith(allowed);
        allowed.UnionWith(parentsAllowed);
        allowed.ExceptWith(denied);
        denied.UnionWith(parentsDenied);

        if (allowed.Count > 0)
            this.cachedAllowed[role] = allowed;
        else
            this.cachedAllowed.Remove(role);

        if (denied.Count > 0)
            this.cachedDenied[role] = denied;
        else
            this.cachedDenied.Remove(role);

        foreach (var child in this.service.GetChildren(role))
        {
            this.CacheRolePermissions(child);
        }
    }
    #endregion

    #region Private fields and constants
    private readonly Dictionary<string, HashSet<string>> cachedAllowed = new Dictionary<string, HashSet<string>>();
    private readonly Dictionary<string, HashSet<string>> cachedDenied = new Dictionary<string, HashSet<string>>();
    private readonly PermissionsService service;
    #endregion
}
