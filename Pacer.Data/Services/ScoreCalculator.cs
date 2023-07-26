public class ScoreCalculator : IScoreCalculator
{
    private const double MinScore = 0;
    private const double MaxScore = 50;
    // MinTime is the fastest 5K time that will be considered, in minutes
    private const double MinTime = 12.5;
    // MaxTime is the slowest 5K time that will be considered, in minutes
    private const double MaxTime = 40;

    private double CalculateScore(double timeInMinutes, double logBase)
    {
        if (timeInMinutes <= MinTime)
        {
            return MaxScore;
        }
        if (timeInMinutes >= MaxTime)
        {
            return MinScore;
        }

        double normalizedTime = (timeInMinutes - MinTime) / (MaxTime - MinTime);

        double score = MaxScore - ((Math.Log(normalizedTime * 9 + 1) / Math.Log(logBase)) * MaxScore);

        score = Math.Max(MinScore, score);
        score = Math.Min(MaxScore, score);

        return Math.Round(score, 2); // Round to 2 decimal places
    }

    /*
        The anaerobic score calculation uses a logarithm base of 20. 
        This base was chosen because anaerobic fitness is significantly impacted by shorter time differences in a 5K run. 
        This base results in a steeper initial decline of the score as the 5K time increases, reflecting the rapid drop-off 
        in anaerobic fitness for slower 5K times.
    */
    public double CalculateInitialAnaerobicScore(TimeSpan fiveKTime)
    {
        double timeInMinutes = fiveKTime.TotalMinutes;
        double logBase = 20;

        return CalculateScore(timeInMinutes, logBase);
    }
    /*
        The aerobic score calculation uses a logarithm base of 10. 
        This base was chosen because aerobic fitness is less sensitive to shorter time differences in a 5K run compared to anaerobic fitness. 
        This base results in a less steep initial decline of the score as the 5K time increases, reflecting the slower drop-off in aerobic 
        fitness for slower 5K times.
    */
    public double CalculateAerobicScoreFrom5KTime(TimeSpan fiveKTime)
    {
        double timeInMinutes = fiveKTime.TotalMinutes;
        double logBase = 10;

        return CalculateScore(timeInMinutes, logBase);
    }
    
    public double CalculateAerobicScoreFromWeeklyMileage(int weeklyMileage)
    {
        double score = 0;
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
        // Scores for above 20 miles
        else
        {
            // Adjusted score values and mileage for the shallow slope section
            double adjustedMinScore = 25;
            double adjustedMinMileage = 20;

            // Calculate the score based on a linear scale
            score = adjustedMinScore + ((weeklyMileage - adjustedMinMileage) / (maxWeeklyMileage - adjustedMinMileage)) * (MaxScore - adjustedMinScore);
        }

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