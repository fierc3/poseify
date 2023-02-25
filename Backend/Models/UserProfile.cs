using System.Collections.Generic;
using System.Text.Json.Serialization;

[Serializable]
public class UserProfile
{
    public string DisplayName { get; set; } = "";

    public string Token { get; set; } = "";

    public string ImageUrl { get; set; } = "";

    [JsonIgnore]
    public string InternalGuid { get; set; } = "";

}