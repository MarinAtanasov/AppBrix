using System;
using System.Linq;

namespace AppBrix.WebApp.Data
{
    public class Book
    {
        public Guid Id { get; set; }

        public string Author { get; set; }
        public string Title { get; set; }
    }
}
