using HotChocolate.Resolvers;
using SimpleApp.Common;
using SimpleApp.Features.Books;

namespace SimpleApp.Features.Authors;

public class BooksResolver(DbContext dbContext) : IResolver<IEnumerable<Book>>
{
    public Task<IEnumerable<Book>> InvokeAsync(IResolverContext context)
    {
        var author = context.Parent<Author>();

        var books = dbContext.GetBooksByAuthorId(author.Id);
        return Task.FromResult(
            books.Select(s => new Book(s.Id, s.Title, s.AuthorId))
        ); 
    }
}