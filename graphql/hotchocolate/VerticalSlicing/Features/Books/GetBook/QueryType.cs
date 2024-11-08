using SimpleApp.Common;
using SimpleApp.Features.Books.GetBooks;

namespace SimpleApp.Features.Books.GetBook;

public class QueryType : FeatureQueryTypeBase
{
    public override void Configure(IObjectTypeDescriptor descriptor)
    {
        descriptor.Field("book")
            .Type<BookType>()
            .Resolve<BookResolver>();
    }
}