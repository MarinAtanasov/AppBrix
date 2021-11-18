using AppBrix.WebApp.Data;
using AppBrix.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace AppBrix.WebApp.Controllers;

[ApiController]
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
    public IEnumerable<Book> Get() => this.app.Get<BooksService>().Get();

    [HttpGet("{id}")]
    public Book? Get(Guid id) => this.app.Get<BooksService>().Get(id);

    [HttpPost]
    public void Post([FromBody] Book book) => this.app.Get<BooksService>().Add(book);

    [HttpPut]
    public void Put([FromBody] Book book) => this.app.Get<BooksService>().Update(book);

    [HttpDelete("{id}")]
    public void Delete(Guid id) => this.app.Get<BooksService>().Delete(id);
    #endregion

    #region Private fields and constants
    private readonly IApp app;
    #endregion
}
