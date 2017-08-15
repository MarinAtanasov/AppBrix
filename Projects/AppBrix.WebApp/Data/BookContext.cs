using AppBrix.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AppBrix.WebApp.Data
{
    public class BooksContext : DbContextBase
    {
        public DbSet<Book> Books { get; set; }
    }
}
