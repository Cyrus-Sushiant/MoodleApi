namespace MoodleApi.Models;

public class UsersData : IDataModel
{
    public List<User>? Users { get; set; }
    public List<Warning>? Warnings { get; set; }
}
