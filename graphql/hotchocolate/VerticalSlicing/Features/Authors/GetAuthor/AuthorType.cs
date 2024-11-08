using SimpleApp.Features.Books;
using SimpleApp.Features.Books.GetBook;

namespace SimpleApp.Features.Authors;

public class AuthorType : ObjectType<Author>
{
    protected override void Configure(IObjectTypeDescriptor<Author> descriptor)
    {
        descriptor.Field(s => s.Books)
            .Type<NonNullType<ListType<BookType>>>()
            .Resolve<BooksResolver>();
    }
}