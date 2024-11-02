namespace Proj.Types;

public class Query { }

public class QueryType : ObjectType<Query>
{
    protected override void Configure(IObjectTypeDescriptor<Query> descriptor)
    {
        descriptor.Field("authors")
            .Type<ListType<AuthorType>>()
            .Resolve(ctx =>
            {
                var authors = ctx.Services.GetRequiredService<DbContext>().GetAuthors();

                return authors.Select(a => new Author(a.Id, a.Name, []));
            });

        descriptor.Field("author")
            .Argument("id", a => a.Type<NonNullType<IntType>>())
            .Type<AuthorType>()
            .Resolve(ctx =>
            {
                var id = ctx.ArgumentValue<int>("id");
                var author = ctx.Services.GetRequiredService<DbContext>().GetAuthorById(id);

                return author is null 
                    ? null 
                    : new Author(author.Id, author.Name, []);
            });

        descriptor.Field("books")
            .Type<ListType<BookType>>()
            .Resolve(ctx =>
            {
                var dbContext = ctx.Services.GetRequiredService<DbContext>();
                var books = dbContext.GetBooks();

                return books.Select(b => new Book(b.Id, b.Title, b.AuthorId));
            });

        descriptor.Field("book")
            .Argument("id", a => a.Type<NonNullType<IntType>>())
            .Type<BookType>()
            .Resolve(ctx =>
            {
                var id = ctx.ArgumentValue<int>("id");
                var dbContext = ctx.Services.GetRequiredService<DbContext>();

                var book = dbContext.GetBookById(id);
                return book is null
                    ? null
                    : new Book(book.Id, book.Title, book.AuthorId);
            });
    }
}