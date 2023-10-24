namespace Core.Models
{
    public class UploadModel
    {
        public IFormFile? FormFile { get; set; }
        public string EstimationName { get; set; } = "nameless";
        public IEnumerable<string> Tags { get; set; } = new List<string>();
    }
}