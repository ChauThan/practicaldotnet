using HotChocolate.Resolvers;
using SimpleApp.Common;

namespace SimpleApp.Features.Books.AddBook;

public class AddBookResolver(DbContext dbContext) : IResolver<Book>
{
    public Task<Book> InvokeAsync(IResolverContext context)
    {
        var input = context.ArgumentValue<AddBookInput>("input");
        var book = new Models.Book(dbContext.GetBooks().Count + 1, input.Title, input.AuthorId);
                
        return Task.FromResult(new Book(book.Id, book.Title, book.AuthorId));
    }
}