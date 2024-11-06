using Enhance1.Resolvers;

namespace Enhance1.Types;

public class QueryType : ObjectType
{
    protected override void Configure(IObjectTypeDescriptor descriptor)
    {
        descriptor.Name(OperationTypeNames.Query);
        
        descriptor.Field("authors")
            .Type<ListType<AuthorType>>()
            .Resolve<AuthorsResolver>();

        descriptor.Field("author")
            .Argument("id", a => a.Type<NonNullType<IntType>>())
            .Type<AuthorType>()
            .Resolve<AuthorResolver>();
    }
}