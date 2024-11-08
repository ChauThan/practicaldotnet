using SimpleApp.Common;

namespace SimpleApp.Features.Authors.GetAuthor;

public class QueryType: FeatureQueryTypeBase
{
    public override void Configure(IObjectTypeDescriptor descriptor)
    {
        descriptor.Field("author")
            .Type<AuthorType>()
            .Resolve<AuthorResolver>();
    }
}