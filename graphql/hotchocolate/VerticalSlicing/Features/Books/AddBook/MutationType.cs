using SimpleApp.Common;
using SimpleApp.Features.Books.GetBook;

namespace SimpleApp.Features.Books.AddBook;

public class MutationType: FeatureMutationTypeBase
{
    public override void Configure(IObjectTypeDescriptor descriptor)
    {
        descriptor.Field("addBook")
            .Type<BookType>()
            .Argument("input", s =>
                s.Type<InputObjectType<AddBookInput>>())
            .Resolve(ctx =>
            {
                var dbContext = ctx.Services.GetRequiredService<DbContext>();
                
                var input = ctx.ArgumentValue<AddBookInput>("input");
                var book = new Models.Book(dbContext.GetBooks().Count + 1, input.Title, input.AuthorId);
                
                return new Book(book.Id, book.Title, book.AuthorId);
            });
    }
}