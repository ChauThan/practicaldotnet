using SpotifyWeb;

namespace MusicMatcher.GraphQL.Types;

[GraphQLDescription("A single audio file, usually a song.")]
public class Track
{
    [ID]
    [GraphQLDescription("The ID for the track.")]
    public string Id { get; }

    [GraphQLDescription("The name of the track.")]
    public string Name { get; set; }

    [GraphQLDescription("The track length in milliseconds.")]
    public double DurationMs { get; set; }

    [GraphQLDescription(
        "Whether or not the track has explicit lyrics (true = yes it does; false = no it does not OR unknown)"
    )]
    public bool Explicit { get; set; }

    [GraphQLDescription("The URI for the track, usually a Spotify link.")]
    public string Uri { get; set; }

    public Track(string id, string name, string uri)
    {
        Id = id;
        Name = name;
        Uri = uri;
    }
    public Track(PlaylistTrackItem obj)
    {
        Id = obj.Id;
        Name = obj.Name;
        DurationMs = obj.Duration_ms;
        Explicit = obj.Explicit;
        Uri = obj.Uri;
    }

}