using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentStorage.Models
{
    public class Solution
    {
        public int Id { get; set; }
        public int CreatorId { get; set; }
        public int AssignmentId { get; set; }
        public string FilePath { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // navigation properties
        [ForeignKey("AssignmentId")]
        public Assignment Assignment { get; set; }
        [ForeignKey("CreatorId")]
        public ApplicationUser Creator { get; set; }
    }
}
