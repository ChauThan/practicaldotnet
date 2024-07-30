using System.ComponentModel.DataAnnotations;
using ConferencePlanner.GraphQL.Types;

namespace ConferencePlanner.GraphQL.Data
{
    public class Speaker
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string? Name { get; set; }

        [StringLength(4000)]
        public string? Bio { get; set; }

        public JobType JobType { get; set; }

        [StringLength(1000)]
        public virtual string? WebSite { get; set; }

        public ICollection<SessionSpeaker> SessionSpeakers { get; set; } = [];
    }

    public enum JobType
    {
        Instructor,

        Presenter,
    }
}