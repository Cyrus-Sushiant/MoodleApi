namespace MoodleApi.Models;

public class Group : IDataModel
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int DescriptionFormat { get; set; }
    public string? EnrolmentKey { get; set; }
}