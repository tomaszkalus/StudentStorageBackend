using StudentStorage.Models.Enums;

namespace StudentStorage.Models.DTO.User;
public class UserAdminDto
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Role { get; set; }
    public DateTime CreatedAt { get; set; }
}
