namespace Proj.Types;

public record Book(int Id, string Title, int AuthorId)
{
    public Author? Author { get; }
};

public class BookType : ObjectType<Book>
{
    protected override void Configure(IObjectTypeDescriptor<Book> descriptor)
    {
        descriptor.Field(s => s.AuthorId)
            .Ignore();
        
        descriptor.Field(s => s.Author)
            .Type<NonNullType<AuthorType>>()
            .Resolve(ctx =>
            {
                var dbContext = ctx.Services.GetRequiredService<DbContext>();

                var book = ctx.Parent<Book>();
                var author = dbContext.GetAuthorById(book.AuthorId);

                if (author is not null)
                {
                    return new Author(author.Id, author.Name, []);    
                }
                
                ctx.ReportError("Author not found");
                return null;
            });
    }
}