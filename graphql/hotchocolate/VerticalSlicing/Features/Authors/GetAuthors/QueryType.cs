using SimpleApp.Common;

namespace SimpleApp.Features.Authors.GetAuthors;

public class QueryType: FeatureQueryTypeBase
{
    public override void Configure(IObjectTypeDescriptor descriptor)
    {
        descriptor.Field("authors")
            .Type<ListType<AuthorType>>()
            .Resolve<AuthorsResolver>();
    }
}