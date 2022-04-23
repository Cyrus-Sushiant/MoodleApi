namespace MoodleApi.Models;

public class Warning
{
    public int ItemId { get; set; }
    public string? Item { get; set; }
    public string? WarningCode { get; set; }
    public string? Message { get; set; }
}