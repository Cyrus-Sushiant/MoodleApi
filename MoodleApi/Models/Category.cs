namespace MoodleApi.Models;

public class Category : IDataModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? IdNumber { get; set; }
    public string? Description { get; set; }
    public int DescriptionFormat { get; set; }
    public int Parent { get; set; }
    public int SortOrder { get; set; }
    public int CourseCount { get; set; }
    public int Visible { get; set; }
    public int VisibleOld { get; set; }
    public int TimeModified { get; set; }
    public int Depth { get; set; }
    public string? Path { get; set; }
    public object? Theme { get; set; }
}