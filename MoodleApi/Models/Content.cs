namespace MoodleApi.Models;

public class Content : IDataModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int Visible { get; set; }
    public string? Summary { get; set; }
    public int SummaryFormat { get; set; }
    public List<Module>? Modules { get; set; }
}