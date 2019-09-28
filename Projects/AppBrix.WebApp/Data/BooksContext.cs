using AppBrix.Data;
using Microsoft.EntityFrameworkCore;

namespace AppBrix.WebApp.Data
{
    public class BooksContext : DbContextBase
    {
        #nullable disable
        public DbSet<Book> Books { get; set; }
        #nullable restore
    }
}
