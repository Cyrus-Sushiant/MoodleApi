namespace MoodleApi.Models;

public class EnrolledUser : IDataModel
{
    public int Id { get; set; }
    public string? UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Department { get; set; }
    public int FirstAccess { get; set; }
    public int LastAccess { get; set; }
    public string? Description { get; set; }
    public int DescriptionFormat { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? ProfileImageUrlSmall { get; set; }
    public string? ProfileImageUrl { get; set; }
    public List<Group>? Groups { get; set; }
    public List<Role>? Roles { get; set; }
    public List<EnrolledCours>? EnrolledCourses { get; set; }
    public List<Preference>? Preferences { get; set; }
}