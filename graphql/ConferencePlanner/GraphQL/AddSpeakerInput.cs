namespace ConferencePlanner.GraphQL.Data
{
    public record AddSpeakerInput(
        string Name,
        string Bio,
        string WebSite);

    public class AddSpeakerPayload(Speaker speaker)
    {
        public Speaker Speaker { get; } = speaker;
    }
}