using AppBrix.Data.Data;
using Microsoft.EntityFrameworkCore;

namespace AppBrix.WebApp.Data;

public class BooksDbContext : DbContextBase
{
    public DbSet<Book> Books { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>().Property(x => x.Author).IsUnicode().HasMaxLength(128);
        modelBuilder.Entity<Book>().Property(x => x.Title).IsUnicode().HasMaxLength(64);
    }
}
