using AppBrix.Data.Services;
using AppBrix.WebApp.Data;

namespace AppBrix;

public static class WebAppExtensions
{
    public static BooksDbContext GetBooksContext(this IDbContextService service) => (BooksDbContext)service.Get(typeof(BooksDbContext));
}
