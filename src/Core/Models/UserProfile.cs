using System.Text.Json.Serialization;

namespace Core.Models
{
    [Serializable]
    public class UserProfile
    {
        public string DisplayName { get; set; } = "";

        public string Token { get; set; } = "";

        public string ImageUrl { get; set; } = "";

        [JsonIgnore]
        public string InternalGuid { get; set; } = "";

    }
}