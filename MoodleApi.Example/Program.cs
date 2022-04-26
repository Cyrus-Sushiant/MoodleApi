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
}

Console.ReadLine();