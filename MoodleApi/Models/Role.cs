namespace MoodleApi.Models;;

public class Role
{
    public int RoleId { get; set; }
    public string? Name { get; set; }
    public string? ShortName { get; set; }
    public int SortOrder { get; set; }
}