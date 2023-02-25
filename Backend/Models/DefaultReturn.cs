using System.Collections.Generic;
using System.Text.Json.Serialization;

[Serializable]
public class DefaultReturn<TData, TError>
{
    public bool Success { get; set; } = false;

    public TData? Data { get; set; }

    public TError? Error { get; set; }


}