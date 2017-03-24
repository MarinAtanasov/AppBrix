using AppBrix.Application;
using AppBrix.WebApp.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBrix.WebApp.Controllers
{
    [Route("api/[controller]")]
    public class BooksController : Controller
    {
        [HttpGet]
        public IEnumerable<Book> Get([FromServices]IApp app)
        {
            using (var context = app.GetDbContextService().Get<BookContext>())
            {
                return context.Books.ToList();
            }
        }

        [HttpGet("{id}")]
        public Book Get([FromServices]IApp app, Guid id)
        {
            using (var context = app.GetDbContextService().Get<BookContext>())
            {
                return context.Books.SingleOrDefault(x => x.Id == id);
            }
        }

        [HttpPost]
        public void Post([FromServices]IApp app, [FromBody]Book book)
        {
            using (var context = app.GetDbContextService().Get<BookContext>())
            {
                context.Books.Add(book);
                context.SaveChanges();
            }
        }

        [HttpPut("{id}")]
        public void Put([FromServices]IApp app, Guid id, [FromBody]Book book)
        {
            using (var context = app.GetDbContextService().Get<BookContext>())
            {
                var original = context.Books.SingleOrDefault(x => x.Id == id);
                original.Author = book.Author;
                original.Title = book.Title;
                context.SaveChanges();
            }
        }

        [HttpDelete("{id}")]
        public void Delete([FromServices]IApp app, Guid id)
        {
            using (var context = app.GetDbContextService().Get<BookContext>())
            {
                var book = context.Books.Single(x => x.Id == id);
                context.Books.Remove(book);
                context.SaveChanges();
            }
        }
    }
}
