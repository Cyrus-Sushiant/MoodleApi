using MoodleApi;

var moodle = new Moodle("https://www.moodle.org/");
var authentiactionResult = await moodle.Login("aminsafaei.info", "123456");
if (authentiactionResult.Succeeded)
{
    var siteInfo = await moodle.GetSiteInfo();

    Console.WriteLine($"Site Name: {siteInfo.Data.SiteName}");
}

Console.ReadLine();
