public interface IScoreCalculator
{
    // Calculate the initial anaerobic score from a 5K time
    double CalculateInitialAnaerobicScore(TimeSpan fiveKTime);
    // Calculate the initial aerobic score from weekly mileage and a 5K time
    double CalculateInitialAerobicScore(int weeklyMileage, TimeSpan fiveKTime);
}