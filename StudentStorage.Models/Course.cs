using System.ComponentModel.DataAnnotations.Schema;

namespace StudentStorage.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string CreatorId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }

        // navigation properties
        [ForeignKey("CreatorId")]
        public ApplicationUser Creator { get; set; }
        public ICollection<Assignment> Assignments { get; } = [];
        public ICollection<Request> Requests { get; } = [];
        public ICollection<ApplicationUser> Students { get; } = [];
    }
}
