namespace BarcodeDecodeLib.Utils.Time;

public interface ITimeConverter
{
    string? ConvertToNeededTimeZoneByFormat(DateTimeOffset? dateTime, string dateTimeFormat);
    string? ConvertToUtcTimeZoneByFormat(DateTimeOffset? dateTime, string dateTimeFormat);
    TimeZoneInfo GetTimeZone();
}