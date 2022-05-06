using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MoodleApi;
using MoodleApi.Extensions;


var builder = new HostBuilder().ConfigureServices((hostContext, services) =>
{
    services.AddHttpClient();
    services.AddMoodleApi();
}).UseConsoleLifetime();

var host = builder.Build();

using var serviceScope = host.Services.CreateScope();
var services = serviceScope.ServiceProvider;
var moodle = services.GetRequiredService<Moodle>();

moodle.SetHost("https://www.moodle.org/");
var authentiactionResult = await moodle.Login("mr.aminsafaei", "123456");
if (authentiactionResult.Succeeded)
{
    var siteInfo = await moodle.GetSiteInfo();
    Console.WriteLine($"Site Name: {siteInfo.Data.SiteName}");

    var studentData = await moodle.GetUsersByField("username", "StudentTest");
    if (studentData.Succeeded)
    {
        var student = studentData.DataArray[0];
        Console.WriteLine($"Student name: {student.FirstName} {student.LastName}");

        var coursesData = await moodle.GetUserCourses(student.Id);
        if (coursesData.Succeeded)
        {
            for (int i = 0; i < coursesData.DataArray.Length; i++)
            {
                var course = coursesData.DataArray[i];
                Console.WriteLine($"{i}. Cource name: {course.DisplayName}");
            }
        }
    }
}

Console.ReadLine();