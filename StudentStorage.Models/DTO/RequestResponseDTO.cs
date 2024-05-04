using StudentStorage.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentStorage.Models.DTO
{
    public class RequestResponseDTO
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string UserId { get; set; }
        public CourseRequestStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public ApplicationUser UserDTO { get; set; }
    }
}
