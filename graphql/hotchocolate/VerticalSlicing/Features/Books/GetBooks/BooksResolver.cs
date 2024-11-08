using HotChocolate.Resolvers;
using SimpleApp.Common;

namespace SimpleApp.Features.Books.GetBooks;

public class BooksResolver(DbContext dbContext) : IResolver<IEnumerable<Book>>
{
    public Task<IEnumerable<Book>> InvokeAsync(IResolverContext context)
    {
        var books = dbContext.GetBooks();

        return Task.FromResult(
            books.Select(b => new Book(b.Id, b.Title, b.AuthorId))
        );
    }
}