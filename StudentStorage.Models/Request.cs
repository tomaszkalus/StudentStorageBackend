using StudentStorage.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentStorage.Models
{
    public class Request
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string UserId { get; set; }
        public CourseRequestStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // navigation properties
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }

    }
}
