using System.ComponentModel.DataAnnotations.Schema;

namespace StudentStorage.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string CreatorId { get; set; }
        [ForeignKey("CreatorId")]
        public ApplicationUser Creator { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Assignment> Assignments { get; } = [];
        public ICollection<Request> Requests { get; } = [];
    }
}
