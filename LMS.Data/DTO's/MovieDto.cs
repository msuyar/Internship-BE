﻿using LMS.Data.Enums;

namespace LMS.Data.Dtos
{
    public class MovieDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Plot { get; set; }
        public string Cast { get; set; }
        public string Director { get; set; }
        public string Category { get; set; }
        public int Duration { get; set; }
        public double Rating { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}

