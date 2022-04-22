using System.Text;

namespace MoodleApi.Extensions
{
    internal static class StringBuilderExtensions
    {
        public static StringBuilder AppendFilterQuery(this StringBuilder stringBuilder, string key, string value)
        {
            return stringBuilder.Append(key).Append(value);
        }

        public static StringBuilder AppendFilterQuery(this StringBuilder stringBuilder, string key, int value)
        {
            return stringBuilder.Append(key).Append(value);
        }

        public static StringBuilder AppendFilterQueryIfHasValue(this StringBuilder stringBuilder, string key, string? value)
        {
            if (value.HasValue())
                return stringBuilder.Append(key).Append(value);
            else
                return stringBuilder;
        }

        public static StringBuilder AppendFilterQueryIfHasValue(this StringBuilder stringBuilder, string key, int? value)
        {
            if (value.HasValue)
                return stringBuilder.Append(key).Append(value);
            else
                return stringBuilder;
        }

        public static StringBuilder AppendFilterQueryIf(this StringBuilder stringBuilder, bool condition, string key, int? value)
        {
            if (condition)
                return stringBuilder.Append(key).Append(value);
            else
                return stringBuilder;
        }
    }
}
