// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Lifecycle;
using AppBrix.Permissions.Services;
using System;
using System.Collections.Generic;

namespace AppBrix.Permissions.Impl;

internal sealed class PermissionsService : IPermissionsService, IApplicationLifecycle
{
    #region IApplicationLifecycle implementation
    public void Initialize(IInitializeContext context)
    {
    }

    public void Uninitialize()
    {
        this.parents.Clear();
        this.children.Clear();
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
            throw new InvalidOperationException($"Role cannot have itself as a parent: {role}.");
        if (this.HasParent(parent, role))
            throw new InvalidOperationException($"Trying to create a circular dependency between {role} and {parent}.");

        this.parents.AddValue(role, parent);
        this.children.AddValue(parent, role);
    }

    public void RemoveParent(string role, string parent)
    {
        if (string.IsNullOrEmpty(role))
            throw new ArgumentNullException(nameof(role));
        if (string.IsNullOrEmpty(parent))
            throw new ArgumentNullException(nameof(parent));

        this.parents.RemoveValue(role, parent);
        this.children.RemoveValue(parent, role);
    }

    public IReadOnlyCollection<string> GetParents(string role)
    {
        if (string.IsNullOrEmpty(role))
            throw new ArgumentNullException(nameof(role));

        return this.parents.GetOrEmpty(role);
    }

    public IReadOnlyCollection<string> GetChildren(string role)
    {
        if (string.IsNullOrEmpty(role))
            throw new ArgumentNullException(nameof(role));

        return this.children.GetOrEmpty(role);
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

        return this.HasPermissionInternal(role, permission);
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
    private bool HasParent(string role, string parent)
    {
        if (this.parents.TryGetValue(role, out var roleParents))
        {
            foreach (var roleParent in roleParents)
            {
                if (roleParent == parent || this.HasParent(roleParent, parent))
                    return true;
            }
        }

        return false;
    }

    private bool HasPermissionInternal(string role, string permission)
    {
        if (this.denied.TryGetValue(role, out var roleDenied) && roleDenied.Contains(permission))
            return false;

        if (this.allowed.TryGetValue(role, out var roleAllowed) && roleAllowed.Contains(permission))
            return true;

        if (this.parents.TryGetValue(role, out var roleParents))
        {
            foreach (var roleParent in roleParents)
            {
                if (this.HasPermissionInternal(roleParent, permission))
                    return true;
            }
        }

        return false;
    }
    #endregion

    #region Private fields and constants
    private readonly Dictionary<string, HashSet<string>> parents = new Dictionary<string, HashSet<string>>();
    private readonly Dictionary<string, HashSet<string>> children = new Dictionary<string, HashSet<string>>();
    private readonly Dictionary<string, HashSet<string>> allowed = new Dictionary<string, HashSet<string>>();
    private readonly Dictionary<string, HashSet<string>> denied = new Dictionary<string, HashSet<string>>();
    #endregion
}
