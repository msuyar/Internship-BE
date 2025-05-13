using System.ComponentModel.DataAnnotations;
using LMS.Data.Enums;
using YourProject.Attributes;

namespace LMS.Data.Dtos
{
    public class FilterMovieDto
    {
        public string? Title { get; set; }
        public string? Plot { get; set; }
        public string? Cast { get; set; }
        public string? Director { get; set; } 
        public string? Category { get; set; }
        [Range(0, 600, ErrorMessage = "Min duration must be between 0 and 600 minutes.")]
        public int? MinDuration { get; set; }
        [Range(0, 600, ErrorMessage = "Max duration must be between 0 and 600 minutes.")]
        public int? MaxDuration { get; set; }
        [Range(0, 10)]
        public float? Rating { get; set; }
        [ReleaseYearRange(1888)]
        public DateTime? ReleaseDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public MovieSortBy SortBy { get; set; } = MovieSortBy.Title; 
        public SortingType SortOrder { get; set; } = SortingType.asc;
    }
}

