// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Configuration;
using AppBrix.Lifecycle;
using AppBrix.Resolve;
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
        /// Gets the id of the application.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the application's configuration.
        /// </summary>
        IAppConfig AppConfig { get; }

        /// <summary>
        /// Gets or sets the currently loaded object resolver.
        /// </summary>
        IResolver Resolver { get; set; }

        /// <summary>
        /// Initializes and starts the application.
        /// </summary>
        void Start();

        /// <summary>
        /// Uninitializes and stops the application
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
