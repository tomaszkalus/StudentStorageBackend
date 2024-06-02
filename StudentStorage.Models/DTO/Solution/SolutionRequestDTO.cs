using Microsoft.AspNetCore.Http;

namespace StudentStorage.Models.DTO.Solution
{
    public class SolutionRequestDTO
    {
        public IFormFile File { get; set; }
        public string Description { get; set; }
    }
}
