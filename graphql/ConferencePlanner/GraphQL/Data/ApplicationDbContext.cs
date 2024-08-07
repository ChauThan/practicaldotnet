using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.GraphQL.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
    {
        public DbSet<Speaker> Speakers { get; set; } = default!;

        public DbSet<Session> Sessions { get; set; } = default!;

        public DbSet<Track> Tracks { get; set; } = default!;

        public DbSet<Attendee> Attendees { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Speaker>()
                .Property(e => e.JobType)
                .HasConversion(
                    v => v.ToString(),
                    v => (JobType)Enum.Parse(typeof(JobType), v)
                );

            modelBuilder
                .Entity<Attendee>()
                .HasIndex(a => a.UserName)
                .IsUnique();

            // Many-to-many: Session <-> Attendee
            modelBuilder
                .Entity<SessionAttendee>()
                .HasKey(ca => new { ca.SessionId, ca.AttendeeId });

            // Many-to-many: Speaker <-> Session
            modelBuilder
                .Entity<SessionSpeaker>()
                .HasKey(ss => new { ss.SessionId, ss.SpeakerId });
        }
    }
}