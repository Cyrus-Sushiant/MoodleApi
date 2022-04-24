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
    public bool Completionhascriteria { get; set; }
    public bool Completionusertracked { get; set; }
    public int Category { get; set; }
    public double? Progress { get; set; }
    public bool? Completed { get; set; }
    public int Startdate { get; set; }
    public int Enddate { get; set; }
    public int Marker { get; set; }
    public int Lastaccess { get; set; }
    public bool Isfavourite { get; set; }
    public bool Hidden { get; set; }
    public List<OverviewFile>? OverviewFiles { get; set; }
    public bool Showactivitydates { get; set; }
    public bool? Showcompletionconditions { get; set; }
}