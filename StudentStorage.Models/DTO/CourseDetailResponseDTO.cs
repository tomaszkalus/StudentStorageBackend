using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentStorage.Models.DTO
{
    public class CourseDetailResponseDTO : CourseResponseDTO
    {
        public IEnumerable<AssignmentResponseDTO> Assignments { get; set; }
    }
}
