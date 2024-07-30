using System.ComponentModel.DataAnnotations;
using ConferencePlanner.GraphQL.Types;

namespace ConferencePlanner.GraphQL.Data
{
    public class Track
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        [UseUpperCase]
        public string? Name { get; set; }


        [UsePaging(typeof(NonNullType<SessionType>))]
        public ICollection<Session> Sessions { get; set; } = [];
    }
}