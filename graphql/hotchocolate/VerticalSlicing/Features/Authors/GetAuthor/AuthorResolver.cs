using HotChocolate.Resolvers;
using SimpleApp.Common;

namespace SimpleApp.Features.Authors;

public class AuthorResolver(DbContext dbContext) : IResolver<Author?>
{
    public Task<Author?> InvokeAsync(IResolverContext context)
    {
        var id = context.ArgumentValue<int>("id");
        var author = dbContext.GetAuthorById(id);

        return Task.FromResult(
            author is null 
                ? null 
                : new Author(author.Id, author.Name));
    }
}