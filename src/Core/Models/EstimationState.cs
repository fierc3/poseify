using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EstimationState
{
    Processing,
    Failed,
    Success,
    Queued
}
