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
    }
}
