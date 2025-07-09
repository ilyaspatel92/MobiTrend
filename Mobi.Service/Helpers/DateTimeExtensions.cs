using Azure.Core;
using Microsoft.AspNetCore.Http;
using TimeZoneConverter;

public static class DateTimeExtensions
{
    public static DateTime ConvertToUserTime(this DateTime utcDateTime, string userTimeZoneId = "")
    {
        // Access HttpContext to get the cookie
        var httpContext = new HttpContextAccessor().HttpContext;

        if (string.IsNullOrEmpty(userTimeZoneId))
            userTimeZoneId = httpContext?.Request?.Cookies["userTimeZone"];

        if (string.IsNullOrEmpty(userTimeZoneId))
            userTimeZoneId = "Asia/Kuwait"; // fallback

        TimeZoneInfo userTimeZone;

        try
        {
            userTimeZone = TZConvert.GetTimeZoneInfo(userTimeZoneId); // IANA to Windows
        }
        catch
        {
            userTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time"); // fallback
        }

        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc), userTimeZone);
    }
}
