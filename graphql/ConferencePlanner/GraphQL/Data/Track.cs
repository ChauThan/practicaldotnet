using System.ComponentModel.DataAnnotations;

namespace ConferencePlanner.GraphQL.Data
{
    public class Track
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        [UseUpperCase]
        public string? Name { get; set; }

        public ICollection<Session> Sessions { get; set; } = [];
    }
}