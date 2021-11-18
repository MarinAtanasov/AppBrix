using AppBrix.Data;
using Microsoft.EntityFrameworkCore;

namespace AppBrix.WebApp.Data;

public class BooksContext : DbContextBase
{
    #nullable disable
    public DbSet<Book> Books { get; set; }
    #nullable restore

    /// <summary>
    /// Configures the creation of the books models.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>().Property(x => x.Author).IsUnicode().HasMaxLength(128);
        modelBuilder.Entity<Book>().Property(x => x.Title).IsUnicode().HasMaxLength(64);
    }
}
