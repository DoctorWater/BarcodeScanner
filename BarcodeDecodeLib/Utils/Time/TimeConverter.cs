using Microsoft.Extensions.Options;

namespace BarcodeDecodeLib.Utils.Time;

public sealed class TimeConverter : ITimeConverter
{
    private readonly TimeZoneSettings _timeZoneSettings;

    public TimeConverter(IOptions<TimeZoneSettings> timeZoneSettings)
    {
        _timeZoneSettings = timeZoneSettings.Value ?? throw new ArgumentNullException(nameof(timeZoneSettings));
    }

    public TimeZoneInfo GetTimeZone()
    {
        return TimeZoneInfo.FindSystemTimeZoneById(_timeZoneSettings.TimeZoneName);
    }

    public string? ConvertToNeededTimeZoneByFormat(DateTimeOffset? dateTime, string dateTimeFormat)
    {
        if (!dateTime.HasValue)
        {
            return null;
        }

        var timeZone = GetTimeZone();

        var correctTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime.Value.UtcDateTime, timeZone);

        return correctTime.ToString(dateTimeFormat);
    }

    public string? ConvertToUtcTimeZoneByFormat(DateTimeOffset? dateTime, string dateTimeFormat)
    {
        if (!dateTime.HasValue)
        {
            return null;
        }

        var timeZone = GetTimeZone();

        var local = DateTime.SpecifyKind(dateTime.Value.LocalDateTime, DateTimeKind.Unspecified);
        var correctTime = TimeZoneInfo.ConvertTimeToUtc(local, timeZone);

        return correctTime.ToString(dateTimeFormat);
    }
}