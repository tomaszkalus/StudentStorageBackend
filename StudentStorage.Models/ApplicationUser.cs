using Microsoft.AspNetCore.Identity;
using StudentStorage.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace StudentStorage.Models
{
    public class ApplicationUser : IdentityUser
    {
        string Email { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        Roles Role { get; set; }
        DateTime CreatedAt { get; set; }
    }
}
