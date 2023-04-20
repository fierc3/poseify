using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Core.Models
{
    [Serializable]
    public class Tag
    {
        public string InternalGuid { get; set; } = "";
        public string DisplayName { get; set; } = "";
    }
}