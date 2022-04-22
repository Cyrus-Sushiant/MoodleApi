using System.Text.Json;

namespace MoodleApi.Extensions
{
    internal static class StringExtensions
    {
        public static bool HasValue(this string? value)
        {
            return string.IsNullOrEmpty(value) is false;
        }

        public static bool HasNoValue(this string? value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool TryParseJson<T>(this string stringJson, out T? result)
        {
            try
            {
                result = stringJson.ParseJson<T>();
                return true;
            }
            catch (Exception)
            {
                result = default(T);
                return false;
            }
        }

        public static T? ParseJson<T>(this string stringJson)
        {
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
            };
            return JsonSerializer.Deserialize<T>(stringJson, options);
        }
    }
}
