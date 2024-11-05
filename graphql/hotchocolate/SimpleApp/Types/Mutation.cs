using HotChocolate.Subscriptions;

namespace Proj.Types;

public class MutationType : ObjectType
{
    protected override void Configure(IObjectTypeDescriptor descriptor)
    {
        descriptor.Name(OperationTypeNames.Mutation);
        
        descriptor.Field("addAuthor")
            .Type<AuthorType>()
            .Argument("input", s =>
                s.Type<InputObjectType<AddAuthor>>())
            .Resolve(async ctx =>
            {
                var dbContext = ctx.Services.GetRequiredService<DbContext>();
                
                var input = ctx.ArgumentValue<AddAuthor>("input");
                var author = new Models.Author(dbContext.GetAuthors().Count + 1, input.Name);
                
                dbContext.AddAuthor(author);

                var result = new Author(author.Id, author.Name, []); 
                var sender = ctx.Service<ITopicEventSender>();
                await sender.SendAsync("authorAdded", result);

                return result;
            });

        descriptor.Field("addBook")
            .Type<BookType>()
            .Argument("input", s =>
                s.Type<InputObjectType<AddBook>>())
            .Resolve(ctx =>
            {
                var dbContext = ctx.Services.GetRequiredService<DbContext>();
                
                var input = ctx.ArgumentValue<AddBook>("input");
                var book = new Models.Book(dbContext.GetBooks().Count + 1, input.Title, input.AuthorId);
                
                return new Book(book.Id, book.Title, book.AuthorId);
            });
    }

    private record AddAuthor(string Name);
    private record AddBook(string Title, int AuthorId);
}