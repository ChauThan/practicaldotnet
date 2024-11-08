using HotChocolate.Resolvers;
using SimpleApp.Common;

namespace SimpleApp.Features.Authors;

public class AuthorsResolver(DbContext dbContext) : IResolver<IEnumerable<Author>>
{
    public Task<IEnumerable<Author>> InvokeAsync(IResolverContext context)
    {
        var authors = dbContext.GetAuthors();

        return Task.FromResult(authors.Select(a => new Author(a.Id, a.Name)));
    }
}