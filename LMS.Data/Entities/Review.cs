using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LMS.Data.Entities
{
    public class Review
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Guid MovieId { get; set; }
        [Required]
        [Range(0, 10)]
        public double Rating { get; set; }
        public string Note { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Required]
        public DateTime UpdatedAt { get; set; }
        [JsonIgnore]
        public User User { get; set; }
        [JsonIgnore]
        public Movie Movie { get; set; }
    }
}