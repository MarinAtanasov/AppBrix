using AppBrix.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Tests.Mocks
{
    /// <summary>
    /// A dummy DB context used during tests.
    /// </summary>
    public sealed class DataItemContextMock : DbContextBase
    {
        /// <summary>
        /// Gets or sets the items in the context.
        /// </summary>
        public DbSet<DataItemMock> Items { get; set; }
    }
}
