using AppBrix.Data.Data;
using Microsoft.EntityFrameworkCore;

namespace AppBrix.WebApp.Data;

public class BooksDbContext : DbContextBase
{
    #nullable disable
    public DbSet<Book> Books { get; set; }
    #nullable restore

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>().Property(x => x.Author).IsUnicode().HasMaxLength(128);
        modelBuilder.Entity<Book>().Property(x => x.Title).IsUnicode().HasMaxLength(64);
    }
}
