using System.Text.Json.Serialization;

namespace Core.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EstimationState
    {
        Processing,
        Failed,
        Success,
        Queued
    }
}