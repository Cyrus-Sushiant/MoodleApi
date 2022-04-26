using System.Text.Json.Serialization;

namespace MoodleApi.Models;

public class User : IDataModel
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
    public string? Auth { get; set; }
    public bool Suspended { get; set; }
    public bool Confirmed { get; set; }
    [JsonPropertyName("lang")]
    public string? Language { get; set; }
    public string? Theme { get; set; }
    public string? Timezone { get; set; }
    public int MailFormat { get; set; }
    public string? Description { get; set; }
    public int DescriptionFormat { get; set; }
    public string? ProfileImageUrlSmall { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? Country { get; set; }
    public List<CustomField>? CustomFields { get; set; }
}