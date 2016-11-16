// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

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
        
        #region Public methods
        [HttpGet]
        public string AppId()
        {
            return this.app.Id.ToString();
        }
        #endregion

        #region Private fields and constants
        private readonly IApp app;
        #endregion
    }
}
