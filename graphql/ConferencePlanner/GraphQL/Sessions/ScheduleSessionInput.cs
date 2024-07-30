namespace ConferencePlanner.GraphQL.Sessions
{
    public class ScheduleSessionInput
    {
        [ID]
        public int SessionId { get; set; }

        [ID]
        public int TrackId { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset EndTime { get; set; }
    }
}