using AppBrix.Data.Services;
using AppBrix.WebApp.Data;

namespace AppBrix;

public static class WebAppExtensions
{
    public static BooksContext GetBooksContext(this IDbContextService service) => (BooksContext)service.Get(typeof(BooksContext));
}
