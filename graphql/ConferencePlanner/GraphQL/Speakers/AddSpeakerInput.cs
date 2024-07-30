using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL.Speakers
{
    public record AddSpeakerInput(
        string Name,
        string? Bio,
        string? WebSite,
        JobType JobType);
}