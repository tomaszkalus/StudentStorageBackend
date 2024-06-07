using StudentStorage.Models.DTO.Solution;

namespace StudentStorage.Models.DTO.User;

public class UserSolutionsSummaryDTO
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string SolutionStatus { get; set; }
}
