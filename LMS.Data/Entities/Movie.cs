using System.ComponentModel.DataAnnotations;
using LMS.Data.Enums;

namespace LMS.Data.Entities
{
    public class Movie
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Title { get; set; }
        public string? Plot { get; set; }
        public string? Cast { get; set; }
        public string? Director { get; set; }
        public string? Category { get; set; }
        public float Rating { get; set; }
        public int Duration { get; set; }
        public DateTime ReleaseDate { get; set; }
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}

