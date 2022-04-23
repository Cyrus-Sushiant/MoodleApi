namespace MoodleApi.Models;

public class Event
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int Format { get; set; }
    public int CourseId { get; set; }
    public int GroupId { get; set; }
    public int UserId { get; set; }
    public int RepeatId { get; set; }
    public string? ModuleName { get; set; }
    public int Instance { get; set; }
    public string? EventType { get; set; }
    public int TimeStart { get; set; }
    public int TimeDuration { get; set; }
    public int Visible { get; set; }
    public string? Uuid { get; set; }
    public int Sequence { get; set; }
    public int TimeModified { get; set; }
    public object? SubscriptionId { get; set; }
}