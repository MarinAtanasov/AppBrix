// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Configuration;
using AppBrix.Container;
using System;
using System.Linq;

namespace AppBrix.Application
{
    /// <summary>
    /// The base interface used for the applications.
    /// </summary>
    public interface IApp
    {
        /// <summary>
        /// Gets or sets the application's service container.
        /// </summary>
        IContainer Container { get; set; }

        /// <summary>
        /// Gets the application's configuration.
        /// </summary>
        IConfigService ConfigService { get; }
        
        /// <summary>
        /// Loads and initializes the application and initializes.
        /// </summary>
        void Start();

        /// <summary>
        /// Uninitializes and unloads the application.
        /// </summary>
        void Stop();

        /// <summary>
        /// Initializes the applicaiton and all of its modules.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Uninitializes the application and all of its modules.
        /// </summary>
        void Uninitialize();
    }
}
