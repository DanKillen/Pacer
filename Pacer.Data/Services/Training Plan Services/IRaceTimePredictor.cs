using Pacer.Data.Entities;

public interface IRaceTimePredictor
{
    /// <summary>
    /// Calculates the estimated time to complete a marathon based on a given 5K time.
    /// </summary>
    TimeSpan CalculateEstimatedMarathonTime(int age, TimeSpan fiveKTime, string gender);
    /// <summary>
    /// Calculates the estimated time to complete a half marathon based on a given 5K time.
    /// </summary>
    TimeSpan CalculateEstimatedHalfMarathonTime(int age, TimeSpan fiveKTime, string gender);
    /// <summary>
    /// Calculates the equivalent marathon pace based on a target time for a different race type.
    /// </summary>
    public TimeSpan CalculateEquivalentMarathonPace(TimeSpan halfMarathonTime, RaceType raceType, int age, string gender);

}