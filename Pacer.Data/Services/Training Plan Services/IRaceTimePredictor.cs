using Pacer.Data.Entities;

public interface IRaceTimePredictor
{
    TimeSpan CalculateEstimatedMarathonTime(TimeSpan fiveKTime);
    TimeSpan CalculateEstimatedHalfMarathonTime(TimeSpan fiveKTime);
    public TimeSpan CalculateEquivalentMarathonPace(TimeSpan raceTime, RaceType raceType);

}