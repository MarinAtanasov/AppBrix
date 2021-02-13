// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using Microsoft.AspNetCore.Mvc;

namespace AppBrix.Web.Server.Tests.Mocks
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
            this.app = app;
        }
        #endregion

        #region Public and overriden methods
        /// <summary>
        /// Returns true to indicate that the service has been reached as expected.
        /// Returns false if the current application is null.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public bool TestConnection() => this.app != null;

        /// <summary>
        /// Returns true to indicate that the service has been reached as expected.
        /// Returns false if the current application is null.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public int TestConnection([FromBody] int number) => number;
        #endregion

        #region Private fields and constants
        private readonly IApp app;
        #endregion
    }
}
