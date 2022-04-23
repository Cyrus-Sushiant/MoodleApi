using System.Text.Json.Serialization;

namespace MoodleApi.Models;

public class Error
{
    [JsonPropertyName("exception")]
    public string? Exception { get; set; }

    [JsonPropertyName("errorcode")]
    public string? ErrorCode { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("debuginfo")]
    public string? DebugInfo { get; set; }
}