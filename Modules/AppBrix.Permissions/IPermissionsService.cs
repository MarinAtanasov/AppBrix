// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System.Collections.Generic;

namespace AppBrix.Permissions
{
    /// <summary>
    /// Service which operates with permissions.
    /// </summary>
    public interface IPermissionsService
    {
        /// <summary>
        /// Adds a child role to the specified role.
        /// Must not create circular dependency.
        /// </summary>
        /// <param name="role">The parent role.</param>
        /// <param name="child">The child role.</param>
        public void AddChild(string role, string child) => this.AddParent(child, role);

        /// <summary>
        /// Adds a parent role to the specified role.
        /// Must not create circular dependency.
        /// </summary>
        /// <param name="role">The child role.</param>
        /// <param name="parent">The parent role.</param>
        void AddParent(string role, string parent);

        /// <summary>
        /// Removes a parent from a role.
        /// </summary>
        /// <param name="role">The child role.</param>
        /// <param name="parent">The parent role to remove.</param>
        void RemoveParent(string role, string parent);

        /// <summary>
        /// Gets the registered parents to a role.
        /// </summary>
        /// <param name="role">The child role.</param>
        /// <returns>The parents of the role.</returns>
        IReadOnlyCollection<string> GetParents(string role);

        /// <summary>
        /// Gets the registered children to a role.
        /// </summary>
        /// <param name="role">The parent role.</param>
        /// <returns>The children of the role.</returns>
        IReadOnlyCollection<string> GetChildren(string role);

        /// <summary>
        /// Allows a role to access a permission.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="permission">The permission.</param>
        void Allow(string role, string permission);

        /// <summary>
        /// Denies a role access to a permission.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="permission">The permission.</param>
        void Deny(string role, string permission);

        /// <summary>
        /// Sets a role permission to be inherited from the role's parents.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="permission">The permission.</param>
        void Unset(string role, string permission);

        /// <summary>
        /// Checks if a role has access to the specified permission.
        /// If the role is not explicitly allowed or denied access,
        /// the role's parents decide whether the role has access to that permission.
        /// If no parent specifies access, default value is used: false.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="permission">The permission.</param>
        /// <returns>Whether the role has access to the permission.</returns>
        bool HasPermission(string role, string permission);

        /// <summary>
        /// Gets a collection of the allowed permission for the role.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns>The allowed permission.</returns>
        IReadOnlyCollection<string> GetAllowed(string role);

        /// <summary>
        /// Gets a collection of the denied permission for the role.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns>The denied permission.</returns>
        IReadOnlyCollection<string> GetDenied(string role);
    }
}
