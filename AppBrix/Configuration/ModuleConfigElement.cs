// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Modules;
using System;
using System.Configuration;
using System.Linq;

namespace AppBrix.Configuration
{
    /// <summary>
    /// A configuration element holding the information about an application module.
    /// </summary>
    public sealed class ModuleConfigElement : ConfigElement
    {
        #region Construction
        /// <summary>
        /// Creates a new element for the selected module.
        /// </summary>
        /// <typeparam name="T">The type of the module.</typeparam>
        /// <returns>The module element.</returns>
        public static ModuleConfigElement Create<T>() where T : IModule
        {
            return ModuleConfigElement.Create(typeof(T));
        }

        /// <summary>
        /// Creates a new element for the selected module.
        /// </summary>
        /// <param name="type">The type of the module.</typeparam>
        /// <returns>The module element.</returns>
        public static ModuleConfigElement Create(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (!typeof(IModule).IsAssignableFrom(type))
                throw new ArgumentException(string.Format("Type {0} is not of type IModule.", type));

            return new ModuleConfigElement()
            {
                Type = type.GetAssemblyQualifiedName(),
            };
        }
        #endregion

        #region Properties
        /// <summary>
        /// Overriding to fix the order in the serialized file.
        /// </summary>
        public override string Key
        {
            get { return base.Key; }
            set { base.Key = value; }
        }

        /// <summary>
        /// Gets or sets the type of the module.
        /// </summary>
        public string Type
        {
            get { return this.Key; }
            set { this.Key = value; }
        }

        /// <summary>
        /// Gets or sets the module's current status.
        /// Changing this property requires application restart.
        /// </summary>
        [ConfigurationProperty("status", DefaultValue = ModuleStatus.Enabled)]
        public ModuleStatus Status
        {
            get { return (ModuleStatus)this["status"]; }
            set { this["status"] = value; }
        }
        #endregion
    }
}
