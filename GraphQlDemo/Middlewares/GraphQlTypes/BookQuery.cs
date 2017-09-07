using GraphQL.Types;
using GraphQlDemo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GraphQlDemo.Middlewares.GraphQlTypes
{
    public class BooksQuery : GraphQlController
    {
        readonly IBookRepository bookRepository;

        public BooksQuery(IBookRepository bookRepository)
        {
            this.bookRepository = bookRepository;

            //Field<BookType>("book",
            //    arguments: new QueryArguments(
            //        new QueryArgument<StringGraphType>() { Name = "isbn" }),
            //    resolve: context =>
            //    {
            //        var id = context.GetArgument<string>("isbn");
            //        return bookRepository.BookByIsbn(id);
            //    });

            //Field<ListGraphType<BookType>>("books",
            //    resolve: context =>
            //    {
            //        return bookRepository.AllBooks();
            //    });
        }
        
        public Models.Book Book(string isbn)
        {
            return bookRepository.BookByIsbn(isbn);
        }

        public IEnumerable<Models.Book> Books()
        {
            return bookRepository.AllBooks();
        }
    }
}
