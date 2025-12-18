using System;

namespace AppBrix.WebApp.Data;

public class Book
{
	public Book()
	{
		this.Author = string.Empty;
		this.Title = string.Empty;
	}

	public Guid Id { get; set; }

	public string Author { get; set; }

	public string Title { get; set; }
}
