using System.Text.Json.Serialization;

namespace MoodleApi.Models;

public class Course : IDataModel
{
    public int Id { get; set; }
    public string? ShortName { get; set; }
    public int CategoryId { get; set; }
    public int CategorySortOrder { get; set; }
    public string? FullName { get; set; }
    public string? DisplayName { get; set; }
    public string? IdNumber { get; set; }
    public string? Summary { get; set; }
    public int SummaryFormat { get; set; }
    public string? Format { get; set; }
    public int ShowGrades { get; set; }
    public int NewsItems { get; set; }
    public int StartDate { get; set; }
    public int NumSections { get; set; }
    public int MaxBytes { get; set; }
    public int ShowReports { get; set; }
    public int Visible { get; set; }
    public int GroupMode { get; set; }
    public int GroupModeForce { get; set; }
    public int DefaultGroupingId { get; set; }
    public int TimeCreated { get; set; }
    public int TimeModified { get; set; }
    public int EnableCompletion { get; set; }
    public int CompletionNotify { get; set; }
    [JsonPropertyName("lang")]
    public string? Language { get; set; }
    public string? ForceTheme { get; set; }
    public List<CourseFormatOption>? CourseFormatOptions { get; set; }
    public int? HiddenSections { get; set; }
}