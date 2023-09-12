using Pacer.Data.Entities;
using Pacer.Utilities;

namespace Pacer.Data.Services;

public class RaceTimePredictor : IRaceTimePredictor
{
    // Known distances in miles
    private const double Distance5K = 3.1;
    private const double MarathonDistance = 26.2188;
    private const double HalfMarathonDistance = 13.1094;

    // Use the correction factors to calculate the corrected pace
    private double CalculateToCorrectedPace(int age, double knownPace, string raceType, string gender)
    {
        Dictionary<int, (double FiveKFactor, double HalfMarathonFactor, double MarathonFactor)> factorsDict =
            (gender == "Male") ? CorrectionFactors.MaleFactors : CorrectionFactors.FemaleFactors;

        if (factorsDict.TryGetValue(age, out var factors))
        {
            double factor = raceType switch
            {
                "5k" => factors.FiveKFactor,
                "halfmar" => factors.HalfMarathonFactor,
                "mar" => factors.MarathonFactor,
                _ => 1.0
            };
            return knownPace * factor;
        }
        return knownPace;
    }
    // Use the correction factors to derive current pace estimation from corrected pace
    private double CalculateFromCorrectedPace(int age, double correctedKnownPace, string targetRaceType, string gender)
    {
        Dictionary<int, (double FiveKFactor, double HalfMarathonFactor, double MarathonFactor)> factorsDict =
            (gender == "Male") ? CorrectionFactors.MaleFactors : CorrectionFactors.FemaleFactors;

        if (factorsDict.TryGetValue(age, out var factors))
        {
            double factor = targetRaceType switch
            {
                "5k" => factors.FiveKFactor,
                "halfmar" => factors.HalfMarathonFactor,
                "mar" => factors.MarathonFactor,
                _ => 1.0
            };
            return correctedKnownPace / factor;
        }
        return correctedKnownPace;
    }
    // Calculate the estimated marathon time based on a 5k time
    public TimeSpan CalculateEstimatedMarathonTime(int age, TimeSpan fiveKTime, string gender)
    {
        double fiveKTimeMinutes = fiveKTime.TotalMinutes;
        double fiveKPace = fiveKTimeMinutes / Distance5K;
        double correctedFiveKPace = CalculateToCorrectedPace(age, fiveKPace, "5k", gender);
        double correctedMarathonPace = CalculateFromCorrectedPace(age, correctedFiveKPace, "mar", gender);
        double estimatedMarathonTimeMinutes = correctedMarathonPace * MarathonDistance;
        return TimeSpan.FromMinutes(estimatedMarathonTimeMinutes);
    }
    // Calculate the estimated half marathon time based on a 5k time
    public TimeSpan CalculateEstimatedHalfMarathonTime(int age, TimeSpan fiveKTime, string gender)
    {
        double fiveKTimeMinutes = fiveKTime.TotalMinutes;
        double fiveKPace = fiveKTimeMinutes / Distance5K;
        double correctedFiveKPace = CalculateToCorrectedPace(age, fiveKPace, "5k", gender);
        double correctedHalfMarathonPace = CalculateFromCorrectedPace(age, correctedFiveKPace, "halfmar", gender);
        double estimatedHalfMarathonTimeMinutes = correctedHalfMarathonPace * HalfMarathonDistance;
        return TimeSpan.FromMinutes(estimatedHalfMarathonTimeMinutes);
    }
    // Calculate the equivalent marathon pace based on a different race's time for pace calculation
    public TimeSpan CalculateEquivalentMarathonPace(TimeSpan targetTime, RaceType raceType, int age, string gender)
    {
        if (raceType == RaceType.Marathon || raceType == RaceType.BMarathon)
        {
            return TimeSpan.FromMinutes(targetTime.TotalMinutes / MarathonDistance);
        }
        double halfMarathonTimeMinutes = targetTime.TotalMinutes;
        double halfMarathonPace = halfMarathonTimeMinutes / HalfMarathonDistance;
        double correctedHalfMarathonPace = CalculateToCorrectedPace(age, halfMarathonPace, "halfmar", gender);
        double correctedMarathonPace = CalculateFromCorrectedPace(age, correctedHalfMarathonPace, "mar", gender);
        return TimeSpan.FromMinutes(correctedMarathonPace);
    }
}