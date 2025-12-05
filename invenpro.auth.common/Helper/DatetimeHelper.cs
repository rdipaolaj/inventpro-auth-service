using invenpro.auth.common.Constants;
using TimeZoneConverter;

namespace invenpro.auth.common.Helper;

public static class DatetimeHelper
{
    public const string TimeZone = "SA Pacific Standard Time";

    public static DateTime Now()
    {
        TimeZoneInfo zoneInfo = TZConvert.GetTimeZoneInfo(TimeZone);
        return TimeZoneInfo.ConvertTime(DateTime.Now, zoneInfo);
    }

    public static DateTime ConvertToSaPacificDatetime(DateTime dateTime)
    {
        TimeZoneInfo zoneInfo = TZConvert.GetTimeZoneInfo(TimeZone);
        return TimeZoneInfo.ConvertTime(dateTime, zoneInfo);
    }

    public static DateTime UtcNow() => DateTime.UtcNow;

    public static string MeridianFormat(string meridian)
    => meridian switch
    {
        "a.m." => DateConstants.AnteMeridian,
        "AM" => DateConstants.AnteMeridian,
        "p.m." => DateConstants.PastMeridian,
        "PM" => DateConstants.PastMeridian,
        _ => meridian
    };

    public static Dictionary<string, string> MonthMap => monthMap;

    private static readonly Dictionary<string, string> monthMap = new()
    {
        { "Jan", "ene" },
        { "Feb", "feb" },
        { "Mar", "mar" },
        { "Apr", "abr" },
        { "May", "may" },
        { "Jun", "jun" },
        { "Jul", "jul" },
        { "Aug", "ago" },
        { "Sep", "sep" },
        { "Oct", "oct" },
        { "Nov", "nov" },
        { "Dec", "dic" }
    };
}