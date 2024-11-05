using HotChocolate.Execution;
using HotChocolate.Subscriptions;

namespace Proj.Types;
public class SubscriptionType : ObjectType
{
    protected override void Configure(IObjectTypeDescriptor descriptor)
    {
        descriptor.Name(OperationTypeNames.Subscription);

        descriptor.Field("authorAdded")
            .Type<AuthorType>()
            .Resolve(ctx => ctx.GetEventMessage<Author>())
            .Subscribe(async ctx =>
            {
                var receiver = ctx.Service<ITopicEventReceiver>();

                ISourceStream stream = await receiver.SubscribeAsync<Author>("authorAdded");

                return stream;
            });

    }
}