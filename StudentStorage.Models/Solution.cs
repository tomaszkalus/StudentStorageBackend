using System.ComponentModel.DataAnnotations.Schema;

namespace StudentStorage.Models
{
    public class Solution
    {
        public int Id { get; set; }
        public int CreatorId { get; set; }
        public int AssignmentId { get; set; }
        public string FilePath { get; set; }
        public int SizeBytes { get; set; }
        public DateTime CreatedAt { get; set; }

        // navigation properties
        [ForeignKey("AssignmentId")]
        public Assignment Assignment { get; set; }
        [ForeignKey("CreatorId")]
        public ApplicationUser Creator { get; set; }
    }
}
