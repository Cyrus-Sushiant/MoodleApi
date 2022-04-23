using System.Text.Json.Serialization;

namespace MoodleApi.Models;

/// <summary>
/// Represents the data associated to the site information
/// </summary>
public class SiteInfo : IDataModel
{
    public string? SiteName { get; set; }
    public string? UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName { get; set; }
    [JsonPropertyName("lang")]
    public string? Language { get; set; }
    public int UserId { get; set; }
    public string? SiteUrl { get; set; }
    public string? UserPictureUrl { get; set; }
    public List<Function>? Functions { get; set; }
    public int DownloadFiles { get; set; }
    public int UploadFiles { get; set; }
    public string? Release { get; set; }
    public string? Version { get; set; }
    public string? MobileCssUrl { get; set; }
    public List<AdvancedFeature>? AdvancedFeatures { get; set; }
    public bool UserCanManageOwnFiles { get; set; }
    public int UserQuota { get; set; }
    public int UserMaxUploadFileSize { get; set; }
    public int UserHomePage { get; set; }
}