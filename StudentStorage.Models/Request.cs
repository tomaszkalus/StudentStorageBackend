using StudentStorage.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentStorage.Models
{
    public class Request
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string UserId { get; set; }
        private CourseRequestStatus _status;
        public CourseRequestStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                UpdatedAt = DateTime.Now;
            }
        }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // navigation properties
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }

    }
}
