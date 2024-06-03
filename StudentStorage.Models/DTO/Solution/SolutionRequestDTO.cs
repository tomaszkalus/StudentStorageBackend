using Microsoft.AspNetCore.Http;

namespace StudentStorage.Models.DTO.Solution
{
    public class SolutionRequestDTO
    {
        public List<IFormFile> Files { get; set; }
    }

}
