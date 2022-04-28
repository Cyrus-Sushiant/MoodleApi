using System.Text.Json.Serialization;

namespace MoodleApi.Models;

public class Cources : IDataModel
{
    public int Id { get; set; }
    public string? ShortName { get; set; }
    public string? FullName { get; set; }
    public string? DisplayName { get; set; }
    public int EnrolledUserCount { get; set; }
    public string? IdNumber { get; set; }
    public int Visible { get; set; }
    public string? Summary { get; set; }
    public int SummaryFormat { get; set; }
    public string? Format { get; set; }
    public bool ShowGrades { get; set; }
    [JsonPropertyName("lang")]
    public string? Language { get; set; }
    public bool Enablecompletion { get; set; }
    public bool CompletionHasCriteria { get; set; }
    public bool CompletionUserTracked { get; set; }
    public int Category { get; set; }
    public double? Progress { get; set; }
    public bool? Completed { get; set; }
    public int StartDate { get; set; }
    public int EndDate { get; set; }
    public int Marker { get; set; }
    public int LastAccess { get; set; }
    public bool IsFavourite { get; set; }
    public bool Hidden { get; set; }
    public List<OverviewFile>? OverviewFiles { get; set; }
    public bool ShowActivityDates { get; set; }
    public bool? ShowCompletionConditions { get; set; }
}