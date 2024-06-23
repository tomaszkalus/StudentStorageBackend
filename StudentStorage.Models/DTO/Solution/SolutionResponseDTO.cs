namespace StudentStorage.Models.DTO.Solution
{
    public class SolutionResponseDTO
    {
        public int Id { get; set; }
        public int SizeBytes { get; set; }
        public string FileName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
