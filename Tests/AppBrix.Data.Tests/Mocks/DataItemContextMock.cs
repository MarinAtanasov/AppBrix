using AppBrix.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AppBrix.Data.Tests.Mocks
{
    internal sealed class DataItemContextMock : DbContextBase
    {
        public DbSet<DataItemMock> Items { get; set; }
    }
}
