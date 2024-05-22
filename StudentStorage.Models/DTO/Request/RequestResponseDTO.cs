using StudentStorage.Models.Enums;

namespace StudentStorage.Models.DTO.Request
{
    public class RequestResponseDTO
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int UserId { get; set; }
        public CourseRequestStatus Status { get; set; }
        public string StatusDescription { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
