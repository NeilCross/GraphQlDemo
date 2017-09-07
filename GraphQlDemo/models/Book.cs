﻿using System.Collections.Generic;

namespace GraphQlDemo.Models
{
    public class Book
    {
        public string Isbn { get; set; }

        public string Name { get; set; }

        public Author Author { get; set; }

        public Publisher Publisher { get; set; }

        public IEnumerable<Book> Similar { get; set; }
    }
}
