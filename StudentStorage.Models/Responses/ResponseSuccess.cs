namespace StudentStorage.Models.Responses
{
    public class ResponseSuccess
    {
        public string Status => "success";
        public object? Data { get; set; }
    }
   
}
