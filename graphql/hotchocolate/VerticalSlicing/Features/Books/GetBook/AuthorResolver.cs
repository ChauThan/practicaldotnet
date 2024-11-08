using HotChocolate.Resolvers;
using SimpleApp.Common;
using SimpleApp.Features.Authors;

namespace SimpleApp.Features.Books;

public class AuthorResolver(DbContext dbContext) : IResolver<Author>
{
    public Task<Author> InvokeAsync(IResolverContext context)
    {
        var book = context.Parent<Book>();
        var author = dbContext.GetAuthorById(book.AuthorId);

        if (author is not null)
        {
            return Task.FromResult(new Author(author.Id, author.Name)); 
        }
                
        context.ReportError("Author not found");
        return Task.FromResult<Author>(null!);
    }
}