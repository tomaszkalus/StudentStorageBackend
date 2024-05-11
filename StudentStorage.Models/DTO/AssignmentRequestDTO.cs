using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentStorage.Models.DTO
{
    /// <summary>
    /// DTO for sending Assignment requests
    /// </summary>
    public class AssignmentRequestDTO
    {
        /// <summary>
        /// Assignment title
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Assignment description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Assignment due date
        /// </summary>
        public DateTime DueDate { get; set; }
        /// <summary>
        /// Boolean that determines if late submissions are allowed
        /// </summary>
        public bool AllowLateSubmissions { get; set; }
        /// <summary>
        /// Boolean that determines if the assignment is hidden from the students
        /// </summary>
        public bool Hidden { get; set; }
    }
}
