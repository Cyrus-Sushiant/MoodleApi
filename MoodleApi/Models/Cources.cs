using System.Text.Json.Serialization;

namespace MoodleApi.Models;

public class Cources : IDataModel
{
    public int Id { get; set; }
    public string? ShortName { get; set; }
    public string? FullName { get; set; }
    public int EnrolledUserCount { get; set; }
    public string? IdNumber { get; set; }
    public int Visible { get; set; }
    public string? Summary { get; set; }
    public int SummaryFormat { get; set; }
    public string? Format { get; set; }
    public bool Showgrades { get; set; }
    [JsonPropertyName("lang")]
    public string? Language { get; set; }
    public bool EnableCompletion { get; set; }
}