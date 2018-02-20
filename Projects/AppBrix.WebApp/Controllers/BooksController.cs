using AppBrix.WebApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBrix.WebApp.Controllers
{
    [Route("api/[controller]")]
    public class BooksController : Controller
    {
        #region Construction
        public BooksController(IApp app)
        {
            this.app = app;
        }
        #endregion

        #region Public and overriden methods
        [HttpGet]
        public IEnumerable<Book> Get()
        {
            using (var context = this.app.GetDbContextService().Get<BooksContext>())
            {
                return context.Books
                    .AsNoTracking()
                    .ToList();
            }
        }

        [HttpGet("{id}")]
        public Book Get(Guid id)
        {
            using (var context = this.app.GetDbContextService().Get<BooksContext>())
            {
                return context.Books
                    .AsNoTracking()
                    .SingleOrDefault(x => x.Id == id);
            }
        }

        [HttpPost]
        public void Post([FromBody]Book book)
        {
            using (var context = this.app.GetDbContextService().Get<BooksContext>())
            {
                context.Books.Add(book);
                context.SaveChanges();
            }
        }

        [HttpPut("{id}")]
        public void Put(Guid id, [FromBody]Book book)
        {
            using (var context = this.app.GetDbContextService().Get<BooksContext>())
            {
                var original = context.Books.Single(x => x.Id == id);
                original.Author = book.Author;
                original.Title = book.Title;
                context.SaveChanges();
            }
        }

        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {
            using (var context = this.app.GetDbContextService().Get<BooksContext>())
            {
                var book = context.Books.Single(x => x.Id == id);
                context.Books.Remove(book);
                context.SaveChanges();
            }
        }
        #endregion

        #region Private fields and constants
        private readonly IApp app;
        #endregion
    }
}
