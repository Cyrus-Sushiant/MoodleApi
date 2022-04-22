using MoodleApi;

var moodle = new Moodle("https://www.moodle.org/");
var authentiactionResult = await moodle.Login("mr.aminsafaei", "123456");
if (authentiactionResult.Succeeded)
{
    var siteInfo = await moodle.GetSiteInfo();
}

Console.WriteLine("Hello, World!");
