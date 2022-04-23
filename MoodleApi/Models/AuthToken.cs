using System.Text.Json.Serialization;

namespace MoodleApi.Models;

public class AuthToken : IDataModel
{
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    [JsonPropertyName("privatetoken")]
    public string? PrivateToken { get; set; }
}