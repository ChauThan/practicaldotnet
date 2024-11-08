using SimpleApp.Common;
using SimpleApp.Features.Books.GetBook;

namespace SimpleApp.Features.Books.GetBooks;

public class QueryType : FeatureQueryTypeBase
{
    public override void Configure(IObjectTypeDescriptor descriptor)
    {
        descriptor.Field("books")
            .Type<ListType<BookType>>()
            .Resolve<BooksResolver>();
    }
}