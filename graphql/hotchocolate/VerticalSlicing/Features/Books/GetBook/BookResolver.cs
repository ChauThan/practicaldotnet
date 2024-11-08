using HotChocolate.Resolvers;
using SimpleApp.Common;

namespace SimpleApp.Features.Books.GetBook;

public class BookResolver(DbContext dbContext) : IResolver<Book?>
{
    public Task<Book?> InvokeAsync(IResolverContext context)
    {
        var id = context.ArgumentValue<int>("id");
        
        var book = dbContext.GetBookById(id);
        return Task.FromResult(
            book is null
            ? null
            : new Book(book.Id, book.Title, book.AuthorId)
        );
    }
}