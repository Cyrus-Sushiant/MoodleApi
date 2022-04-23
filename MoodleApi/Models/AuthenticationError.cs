using System.Text.Json.Serialization;

namespace MoodleApi.Models;

public class AuthenticationError : IDataModel
{
    [JsonPropertyName("error")]
    public string? Error { get; set; }

    [JsonPropertyName("stacktrace")]
    public object? StackTrace { get; set; }

    [JsonPropertyName("debuginfo")]
    public object? DebugInfo { get; set; }

    [JsonPropertyName("reproductionlink")]
    public object? ReproductionLink { get; set; }
}