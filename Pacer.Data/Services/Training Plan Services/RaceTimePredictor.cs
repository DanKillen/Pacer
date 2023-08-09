using Pacer.Data.Entities;

public class RaceTimePredictor : IRaceTimePredictor
{
    // Known distance for 5K in miles
    private const double Distance5K = 3.1;
    
    // Marathon and Half Marathon distances in miles
    private const double MarathonDistance = 26.2;
    private const double HalfMarathonDistance = 13.1;

    // Riegel formula coefficient
    private const double RiegelCoefficient = 1.06;

    private double CalculateEstimatedTime(double knownTime, double knownDistance, double targetDistance)
    {
        // Using the Riegel formula
        return knownTime * Math.Pow((targetDistance / knownDistance), RiegelCoefficient);
    }

    public TimeSpan CalculateEstimatedMarathonTime(TimeSpan fiveKTime)
    {
        double timeInMinutes = fiveKTime.TotalMinutes;
        double estimatedMarathonTimeInMinutes = CalculateEstimatedTime(timeInMinutes, Distance5K, MarathonDistance);
        return TimeSpan.FromMinutes(estimatedMarathonTimeInMinutes);
    }

    public TimeSpan CalculateEstimatedHalfMarathonTime(TimeSpan fiveKTime)
    {
        double timeInMinutes = fiveKTime.TotalMinutes;
        double estimatedHalfMarathonTimeInMinutes = CalculateEstimatedTime(timeInMinutes, Distance5K, HalfMarathonDistance);
        return TimeSpan.FromMinutes(estimatedHalfMarathonTimeInMinutes);
    }
    
    public TimeSpan CalculateEquivalentMarathonPace(TimeSpan raceTime, RaceType raceType)
    {
        TimeSpan equivalentMarathonTime = CalculateEquivalentMarathonTime(raceTime, raceType);
        return TimeSpan.FromMinutes(equivalentMarathonTime.TotalMinutes / MarathonDistance);
    }

    private TimeSpan CalculateEquivalentMarathonTime(TimeSpan raceTime, RaceType raceType)
    {
        if (raceType == RaceType.Marathon || raceType == RaceType.BMarathon)
        {
            return raceTime;
        }
        double estimatedMarathonTimeInSeconds = CalculateEstimatedTime(raceTime.TotalSeconds, HalfMarathonDistance, MarathonDistance);
        return TimeSpan.FromSeconds(estimatedMarathonTimeInSeconds);
    }
}