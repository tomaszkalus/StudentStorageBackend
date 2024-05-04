using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentStorage.Models.DTO
{
    public class AssignmentResponseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime DueDate { get; set; }
        public bool AllowLateSubmissions { get; set; }
        public bool Hidden { get; set; }
        public CourseResponseDTO Course { get; set; }
    }
}
