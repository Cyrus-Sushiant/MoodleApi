namespace MoodleApi.Models;

public class Module
{
    public int Id { get; set; }
    public string? Bame { get; set; }
    public int Visible { get; set; }
    public string? ModIcon { get; set; }
    public string? ModName { get; set; }
    public string? ModPlural { get; set; }
    public string? Availability { get; set; }
    public int Indent { get; set; }
    public string? Url { get; set; }
}