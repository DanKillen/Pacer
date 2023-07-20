public interface IScoreCalculator
{
    double CalculateInitialAnaerobicScore(TimeSpan fiveKTime);
    double CalculateInitialAerobicScore(int weeklyMileage, TimeSpan fiveKTime);
}

public class ScoreCalculator : IScoreCalculator
{
    public double CalculateInitialAnaerobicScore(TimeSpan fiveKTime)
    {
        // Convert the TimeSpan to total seconds for more precision
        double timeInSeconds = fiveKTime.TotalSeconds;

        // Set up the maximum and minimum time and score values for scaling
        double minTimeInSeconds = 12.5 * 60; // 12.5 minutes in seconds
        double maxTimeInSeconds = 40 * 60; // 40 minutes in seconds
        double minScore = 0;
        double maxScore = 50;

        // Ensure the time is within the valid range
        if (timeInSeconds < minTimeInSeconds)
        {
            return maxScore;
        }
        if (timeInSeconds > maxTimeInSeconds)
        {
            return minScore;
        }
        // Normalize the time within the range [0, 1]
        double normalizedTime = (timeInSeconds - minTimeInSeconds) / (maxTimeInSeconds - minTimeInSeconds);

        // Use a logarithmic function to calculate the score, ensuring it's within the range [minScore, maxScore]
        double score = maxScore - ((Math.Log(normalizedTime * 9 + 1) / Math.Log(20)) * maxScore);

        // Ensure the score is within the valid range
        score = Math.Max(minScore, score);
        score = Math.Min(maxScore, score);

        return Math.Round(score, 2); // Round to 2 decimal places
    }
    public double CalculateAerobicScoreFromWeeklyMileage(int weeklyMileage)
    {
        double score = 0;
        double maxAerobicScore = 50;
        double maxWeeklyMileage = 120;

        // Scores for 20 miles and below
        if (weeklyMileage <= 20)
        {
            // Adjusted score values and mileage for the steep slope section
            double adjustedMaxScore = 25;
            double adjustedMaxMileage = 20;

            // Calculate the score based on a linear scale
            score = 10 + (weeklyMileage / adjustedMaxMileage) * (adjustedMaxScore - 10);
        }
        // Scores for above 30 miles
        else
        {
            // Adjusted score values and mileage for the shallow slope section
            double adjustedMinScore = 25;
            double adjustedMinMileage = 20;

            // Calculate the score based on a linear scale
            score = adjustedMinScore + ((weeklyMileage - adjustedMinMileage) / (maxWeeklyMileage - adjustedMinMileage)) * (maxAerobicScore - adjustedMinScore);
        }

        return Math.Round(score, 2); // Round to 2 decimal places
    }

    public double CalculateAerobicScoreFrom5KTime(TimeSpan fiveKTime)
    {
        double maxAerobicScore = 50;
        double minFiveKTime = 12.5;
        double maxFiveKTime = 40;

        // Convert fiveKTime to total minutes for comparison
        double timeInMinutes = fiveKTime.TotalMinutes;

        // Ensure the time is within the valid range
        if (timeInMinutes <= minFiveKTime)
        {
            return maxAerobicScore;
        }
        if (timeInMinutes >= maxFiveKTime)
        {
            return 0;
        }

        // Normalize the time within the range [0, 1]
        double normalizedTime = (timeInMinutes - minFiveKTime) / (maxFiveKTime - minFiveKTime);

        // Using a logarithmic function to calculate the score, ensuring it's within the range [0, maxScore]
        double score = maxAerobicScore - ((Math.Log(normalizedTime * 9 + 1) / Math.Log(10)) * maxAerobicScore);

        // Ensure the score is within the valid range
        score = Math.Max(0, score);
        score = Math.Min(maxAerobicScore, score);

        return Math.Round(score, 2); // Round to 2 decimal places
    }

    public double CalculateInitialAerobicScore(int weeklyMileage, TimeSpan fiveKTime)

    {
        double mileageAerobicScore = CalculateAerobicScoreFromWeeklyMileage(weeklyMileage);
        double timeAerobicScore = CalculateAerobicScoreFrom5KTime(fiveKTime);

        // Return the highest of the two scores
        return Math.Max(mileageAerobicScore, timeAerobicScore);
    }

}