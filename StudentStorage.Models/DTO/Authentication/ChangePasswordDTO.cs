namespace StudentStorage.Models.DTO.Authentication;

public class ChangePasswordDTO
{
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}
