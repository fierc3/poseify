using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Core.Models
{
    [Serializable]
    public class Estimation
    {
        public string InternalGuid { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public IEnumerable<string>? Tags { get; set; } = new List<string>();
        public string UploadingProfile { get; set; } = "";
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
        public EstimationState State { get; set; }
        public string StateText { get; set; } = string.Empty;
    }
}