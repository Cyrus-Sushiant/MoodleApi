namespace MoodleApi.Models;

public class NewUser : IDataModel
{
    public int Id { get; set; }
    public string? UserName { get; set; }
}