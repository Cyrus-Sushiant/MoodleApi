using Microsoft.Extensions.DependencyInjection;

namespace MoodleApi.Extensions
{
    public static class MoodleApiServiceCollectionExtensions
    {
        public static IServiceCollection AddMoodleApi(this IServiceCollection services)
        {
            services.AddScoped<Moodle>();
            return services;
        }
    }
}
