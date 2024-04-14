using AppBrix.Lifecycle;
using AppBrix.WebApp.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBrix.WebApp.Services;

public class BooksService : IApplicationLifecycle
{
    #region Public and overriden methods
    public void Initialize(IInitializeContext context)
    {
        this.app = context.App;
    }

    public void Uninitialize()
    {
        this.app = null!;
    }

    public IEnumerable<Book> Get()
    {
        using var context = this.app.GetDbContextService().GetBooksContext();
        return context.Books
            .AsNoTracking()
            .ToList();
    }

    public Book? Get(Guid id)
    {
        using var context = this.app.GetDbContextService().GetBooksContext();
        return context.Books
            .AsNoTracking()
            .SingleOrDefault(x => x.Id == id);
    }

    public void Add(Book book)
    {
        using var context = this.app.GetDbContextService().GetBooksContext();
        context.Books.Add(book);
        context.SaveChanges();
    }

    public void Update(Book book)
    {
        using var context = this.app.GetDbContextService().GetBooksContext();
        var item = new Book() { Id = book.Id };
        context.Books.Attach(item);
        item.Author = book.Author;
        item.Title = book.Title;
        context.SaveChanges();
    }

    public void Delete(Guid id)
    {
        using var context = this.app.GetDbContextService().GetBooksContext();
        var book = new Book() { Id = id };
        context.Books.Attach(book);
        context.Books.Remove(book);
        context.SaveChanges();
    }
    #endregion

    #region Private fields and constants
    private IApp app = null!;
    #endregion
}
