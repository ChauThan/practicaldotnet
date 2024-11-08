using SimpleApp.Common;

namespace SimpleApp.Features.Authors.AddAuthor;

public class MutationType : FeatureMutationTypeBase
{
    public override void Configure(IObjectTypeDescriptor descriptor)
    {
        descriptor.Field("addAuthor")
            .Type<AuthorType>()
            .Argument("input", s =>
                s.Type<InputObjectType<AddAuthorInput>>())
            .Resolve<AddAuthorResolver>();
    }
}