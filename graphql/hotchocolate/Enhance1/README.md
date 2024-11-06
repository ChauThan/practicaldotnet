## Decoupling Resolvers from Type Definitions
We can effortlessly decouple the resolver from the type definition. 
For instance:
- Create a Resolver class
```csharp
public class AuthorsResolver(DbContext dbContext) : IResolver<IEnumerable<Author>>
{
    public Task<IEnumerable<Author>> InvokeAsync(IResolverContext context)
    {
        var authors = dbContext.GetAuthors();

        return Task.FromResult(authors
            .Select(a => new Author(a.Id, a.Name)));
    }
}
```
- In the type definition, simply declare the resolver.
```csharp
descriptor.Field("authors")
    .Type<ListType<AuthorType>>()
    .Resolve<AuthorsResolver>();
```