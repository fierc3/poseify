using System.Collections.Generic;
using System.Text.Json.Serialization;

[Serializable]
public class DefaultError
{
    public string ErrorCode { get; set; }  = "";
    public string ErrorText { get; set; } = "";

}