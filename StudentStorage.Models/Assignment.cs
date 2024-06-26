﻿using System.ComponentModel.DataAnnotations.Schema;

namespace StudentStorage.Models
{
    public class Assignment
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime DueDate { get; set; }
        public bool AllowLateSubmissions { get; set; }
        public bool Hidden { get; set; }

        // navigation properties
        [ForeignKey("CourseId")]
        public Course Course { get; set; }
        public ICollection<Solution> Solutions { get; } = [];
    }
}
