using ConferencePlanner.GraphQL.Data;
using HotChocolate.Configuration;
using HotChocolate.Types.Descriptors.Definitions;

namespace ConferencePlanner.GraphQL
{
    public class NodeResolverTypeInterceptor : TypeInterceptor
    {
        public override void OnAfterCompleteName(
            ITypeCompletionContext completionContext,
            DefinitionBase definition)
        {
            if (completionContext is not { IsIntrospectionType: false, IsDirective: false })
            {
                return;
            }

            switch (definition)
            {
                case InterfaceTypeDefinition type:
                    MapNodeTypeResolver(type);
                    break;
            }
        }

        private void MapNodeTypeResolver(InterfaceTypeDefinition definition)
        {
            if (definition.RuntimeType == typeof(INode))
            {
                definition.ResolveAbstractType = (context, result) =>
                {
                    if (result is Speaker speaker)
                    {
                        if (context.Schema.TryGetType<ObjectType>(speaker.JobType.ToString(), out var type))
                        {
                            return type;
                        }
                    }

                    return null;
                };
            }

        }
    }
}