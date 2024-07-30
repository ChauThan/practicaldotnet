using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.DataLoader;

namespace ConferencePlanner.GraphQL;

[ExtendObjectType("Subscription")]
public class SessionSubscriptions
{
    [Subscribe]
    [Topic]
    public Task<Session> OnSessionScheduledAsync(
                [EventMessage] int sessionId,
                SessionByIdDataLoader sessionById,
                CancellationToken cancellationToken) =>
                sessionById.LoadAsync(sessionId, cancellationToken);
}
