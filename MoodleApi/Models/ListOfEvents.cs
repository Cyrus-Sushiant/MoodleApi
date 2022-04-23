namespace MoodleApi.Models;

public class ListOfEvents : IDataModel
{
    public List<Event>? Events { get; set; }
    public List<Warning>? Warnings { get; set; }
}