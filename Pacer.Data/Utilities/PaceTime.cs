using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Pacer.Data.Utilities;
// PaceTime is a struct that represents a pace in minutes and seconds per mile.
public readonly struct PaceTime
{
    public PaceTime(int minutes, int seconds)
    {
        if (minutes < 0)
            throw new ArgumentException("Minutes cannot be negative.", nameof(minutes));
        if (seconds < 0 || seconds >= 60)
            throw new ArgumentException("Seconds must be between 0 and 59.", nameof(seconds));

        Minutes = minutes;
        Seconds = seconds;
    }
    public int Minutes { get; }
    public int Seconds { get; }

    public PaceTime(TimeSpan timeSpan)
    {
        if (timeSpan.TotalMinutes < 0)
            throw new ArgumentException("TimeSpan represents a negative pace.", nameof(timeSpan));
        if (timeSpan.Hours > 0)
            throw new ArgumentException("TimeSpan represents more than an hour.", nameof(timeSpan));

        Minutes = timeSpan.Minutes;
        Seconds = timeSpan.Seconds;
    }
    public override string ToString()
    {
        return $"{Minutes}:{Seconds:D2}";
    }
}
