// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using Microsoft.AspNetCore.Mvc;

namespace AppBrix.Web.Server.Tests.Mocks
{
    /// <summary>
    /// A simple controller which can be used to get the current application's id.
    /// </summary>
    [Route("api/[controller]")]
    public sealed class AppIdController : Controller
    {
        #region Construction
        /// <summary>
        /// Creates a new instance of <see cref="AppIdController"/>.
        /// </summary>
        /// <param name="app">The current app.</param>
        public AppIdController(IApp app)
        {
            this.app = app;
        }
        #endregion

        #region Public and overriden methods
        [HttpGet]
        public AppIdMessage AppId() => new AppIdMessage
        {
            Duration = TimeSpan.FromMilliseconds(5),
            Id = ((AppIdConfig)this.app.ConfigService.Get(typeof(AppIdConfig))).Id,
            Time = new DateTime(2020, 2, 2, 2, 2, 2, DateTimeKind.Utc),
            Version = this.GetType().Assembly.GetName().Version
        };
        #endregion

        #region Private fields and constants
        private readonly IApp app;
        #endregion
    }
}
