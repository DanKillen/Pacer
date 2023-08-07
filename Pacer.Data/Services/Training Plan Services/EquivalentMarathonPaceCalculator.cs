
using Pacer.Data.Entities;

public class EquivalentMarathonPaceCalculator
{
    public TimeSpan CalculateEquivalentMarathonPace(TimeSpan raceTime, RaceType raceType)
    {
        TimeSpan equivalentMarathonTime = CalculateEquivalentMarathonTime(raceTime, raceType);
        TimeSpan equivalentMarathonPace = TimeSpan.FromMinutes(equivalentMarathonTime.TotalMinutes / 26.2188);
        return equivalentMarathonPace;
    }
    
    private TimeSpan CalculateEquivalentMarathonTime(TimeSpan raceTime, RaceType raceType)
    {
        if (raceType == RaceType.Marathon)
        {
            return raceTime;
        }
        double[] fiveKTimes = new double[] { 37 * 60, 31.72 * 60, 27.75 * 60, 24.67 * 60, 22.2 * 60, 20.18 * 60, 18.5 * 60, 17.07 * 60, 15.85 * 60, 14.8 * 60, 13.87 * 60, 13.05 * 60, 12.33 * 60 };
        double[] tenKTimes = new double[] { 77.68 * 60, 66.58 * 60, 58.27 * 60, 51.78 * 60, 46.6 * 60, 42.37 * 60, 38.83 * 60, 35.83 * 60, 33.28 * 60, 31.07 * 60, 29.12 * 60, 27.4 * 60, 25.9 * 60 };
        double[] halfMarathonTimes = new double[] { 172.7 * 60, 148.03 * 60, 129.52 * 60, 115.13 * 60, 103.62 * 60, 94.2 * 60, 86.2 * 60, 79.67 * 60, 74.0 * 60, 69.08 * 60, 64.72 * 60, 60.92 * 60, 57.57 * 60 };
        double[] marathonTimes = new double[] { 362.52 * 60, 310.75 * 60, 271.58 * 60, 241.68 * 60, 217.52 * 60, 197.75 * 60, 181.27 * 60, 167.22 * 60, 155.3 * 60, 145.02 * 60, 135.87 * 60, 127.87 * 60, 120.83 * 60 };
        double[] targetTimes = raceType switch
        {
            RaceType.FiveK => fiveKTimes,
            RaceType.TenK => tenKTimes,
            RaceType.HalfMarathon => halfMarathonTimes,
            _ => throw new ArgumentException($"Unsupported race type: {raceType}"),
        };
        double targetTime = raceTime.TotalSeconds;

        // Find the nearest times in the array
        int index = Array.BinarySearch(targetTimes, targetTime);
        if (index < 0)
        {
            index = ~index;
        }

        // Check boundaries of the array
        if (index == 0)
        {
            index = 1;
        }
        else if (index == targetTimes.Length)
        {
            index = targetTimes.Length - 1;
        }

        // Interpolation
        double marathonTimeEstimate = marathonTimes[index - 1] + (targetTime - targetTimes[index - 1]) * (marathonTimes[index] - marathonTimes[index - 1]) / (targetTimes[index] - targetTimes[index - 1]);

        return TimeSpan.FromSeconds(marathonTimeEstimate);
    }
}