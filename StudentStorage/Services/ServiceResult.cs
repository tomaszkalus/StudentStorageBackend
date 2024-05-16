namespace StudentStorage.Services
{
    public class ServiceResult(bool success, string message)
    {
        public bool Success { get; set; } = success;
        public string Message { get; set; } = message;
    }
}
