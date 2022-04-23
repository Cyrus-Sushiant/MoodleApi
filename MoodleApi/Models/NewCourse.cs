namespace MoodleApi.Models;

public class NewCourse : IDataModel
{
    public int Id { get; set; }
    public string? Shortname { get; set; }
}