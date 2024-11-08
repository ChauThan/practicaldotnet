using HotChocolate.Resolvers;
using SimpleApp.Common;

namespace SimpleApp.Features.Authors.AddAuthor;

public class AddAuthorResolver(DbContext dbContext) : IResolver<Author>
{
    public Task<Author> InvokeAsync(IResolverContext context)
    {     
        var input = context.ArgumentValue<AddAuthorInput>("input");
        var author = new Models.Author(dbContext.GetAuthors().Count + 1, input.Name);
                
        dbContext.AddAuthor(author);
        
        return Task.FromResult(new Author(author.Id, author.Name));
    }
}