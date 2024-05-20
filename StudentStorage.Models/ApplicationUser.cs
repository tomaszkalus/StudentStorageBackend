using Microsoft.AspNetCore.Identity;

namespace StudentStorage.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; }

        // navigation properties
        public ICollection<Course> Courses { get; } = new List<Course>();
        public ICollection<Request> Requests { get; } = new List<Request>();
    }
}
