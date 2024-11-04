namespace Proj.Types;

public record Author(int Id, string Name, ICollection<Book> Books);

public class AuthorType : ObjectType<Author>
{
    protected override void Configure(IObjectTypeDescriptor<Author> descriptor)
    {
        descriptor.Field(s => s.Books)
            .Type<NonNullType<ListType<BookType>>>()
            .Resolve(ctx =>
            {
                var author = ctx.Parent<Author>();
                var dbContext = ctx.Services.GetRequiredService<DbContext>();

                var books = dbContext.GetBooksByAuthorId(author.Id);
                return books.Select(s => new Book(s.Id, s.Title, s.AuthorId));
            });
    }
    
}