using SimpleApp.Features.Authors;

namespace SimpleApp.Features.Books.GetBook;

public class BookType: ObjectType<Book>
{
    protected override void Configure(IObjectTypeDescriptor<Book> descriptor)
    {
        descriptor.Field(s => s.AuthorId)
            .Ignore();

        descriptor.Field(s => s.Author)
            .Type<NonNullType<AuthorType>>()
            .Resolve<AuthorResolver>();
    }
}