// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace AppBrix.Web.Server.Tests
{
    /// <summary>
    /// A simple controller which can be used to test the connection to the application.
    /// </summary>
    [Route("api/[controller]")]
    public sealed class TestConnectionController : Controller
    {
        #region Construction
        /// <summary>
        /// Creates a new instance of <see cref="TestConnectionController"/>.
        /// </summary>
        /// <param name="app">The current app.</param>
        public TestConnectionController(IApp app)
        {
        }
        #endregion
        
        #region Public methods
        /// <summary>
        /// Returns true to indicate that the service has been reached as expected.
        /// Returns false if the current application is null.
        /// </summary>
        /// <returns></returns>
        [HttpDelete, HttpGet, HttpHead, HttpPatch, HttpPost, HttpPut]
        public bool TestConnection()
        {
            return true;
        }
        #endregion
    }
}
