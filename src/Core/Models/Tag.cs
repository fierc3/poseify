using System.Collections.Generic;
using System.Text.Json.Serialization;

[Serializable]
public class Tag
{
    public string InternalGuid { get; set; } = "";
    public string Name { get; set; } = "";
}

