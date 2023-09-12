using Pacer.Data.Entities;

namespace Pacer.Data.Services;
public interface IRaceTimePredictor
{

    // Calculates the estimated time to complete a marathon based on a given 5K time.
    TimeSpan CalculateEstimatedMarathonTime(int age, TimeSpan fiveKTime, string gender);
    // Calculates the estimated time to complete a half marathon based on a given 5K time.
    TimeSpan CalculateEstimatedHalfMarathonTime(int age, TimeSpan fiveKTime, string gender);
    /// Calculates the equivalent marathon pace based on a target time for a different race type.
    public TimeSpan CalculateEquivalentMarathonPace(TimeSpan halfMarathonTime, RaceType raceType, int age, string gender);

}