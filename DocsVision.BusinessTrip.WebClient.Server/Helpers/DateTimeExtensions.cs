using System;
using System.Globalization;

namespace ServerExtension.Helpers
{
    public static class DateTimeExtensions
    {
        public static bool TryParseStandardDate(this string dateString, out DateTime parsedDate)
        {
            return DateTime.TryParseExact(
                dateString, "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out parsedDate);
        }

    }
}
