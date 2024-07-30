using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.DataLoader;
using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.GraphQL
{
    [ExtendObjectType("Query")]
    public class SpeakerQueries
    {
        [UsePaging]
        public Task<List<Speaker>> GetSpeakersAsync(ApplicationDbContext context) =>
            context.Speakers.ToListAsync();

        public Task<Speaker> GetSpeakerByIdAsync(
            [ID(nameof(Speaker))] int id,
            SpeakerByIdDataLoader dataLoader,
            CancellationToken cancellationToken) =>
                dataLoader.LoadAsync(id, cancellationToken);

        public async Task<IEnumerable<Speaker>> GetSpeakersByIdAsync(
           [ID(nameof(Speaker))] int[] ids,
           SpeakerByIdDataLoader dataLoader,
           CancellationToken cancellationToken) =>
            await dataLoader.LoadAsync(ids, cancellationToken);
    }
}