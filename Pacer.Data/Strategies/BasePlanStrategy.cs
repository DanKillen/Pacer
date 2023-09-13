using Microsoft.Extensions.Logging;
using Pacer.Data.Entities;
using Pacer.Data.Utilities;
using Pacer.Data.Services;


namespace Pacer.Data.Strategies;
public abstract class BasePlanStrategy
{
    protected enum RunType
    {
        E,  // Easy Run
        R,  // Recovery Run
        X,  // Rest
        I,  // Interval Training
        T, // Tempo Run
        V, // VO2 Max
        L,   // Long Run
        M  // Race Pace

    }
    protected string[] WeekPlans;
    protected readonly DateTime RaceDate;
    protected readonly TimeSpan TargetTime;
    protected readonly ILogger _logger;


    protected BasePlanStrategy(DateTime raceDate, TimeSpan targetTime)
    {
        RaceDate = raceDate;
        TargetTime = targetTime;
    }

    // Generates the workouts for the selected training plan
    public virtual Workout[] GenerateWorkouts()
    {
        var workouts = new List<Workout>();
        DateTime currentWeekStart = RaceDate.AddDays(-7 * WeekPlans.Length);

        for (int week = 0; week < WeekPlans.Length; week++)
        {
            try
            {
                workouts.AddRange(CreateWorkoutsForWeek(WeekPlans[week], currentWeekStart));
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"An error occurred during the parsing of week {week + 1}: {ex.Message}");
            }

            currentWeekStart = currentWeekStart.AddDays(7);
        }

        return workouts.ToArray();
    }
    // Creates a workout object
    protected Workout CreateWorkout(WorkoutType type, DateTime date, double targetDistance, string description = null)
    {
        description ??= $"Run {targetDistance} miles at target pace";
        return new Workout
        {
            Type = type,
            Date = date,
            TargetDistance = targetDistance,
            WorkoutDescription = description
        };
    }
    // Creates a list of workouts for a given week
    protected IEnumerable<Workout> CreateWorkoutsForWeek(string weekPlan, DateTime weekStart)
    {
        var workouts = new List<Workout>();
        string[] dailyPlans = weekPlan.Split(';');

        for (int i = 0; i < dailyPlans.Length; i++)
        {
            string dailyPlan = dailyPlans[i].Trim();

            if (dailyPlan == "X")
            {
                continue;
            }
            if (!TryParseDailyPlan(dailyPlan, out RunType runType, out double distance, out string description))
            {
                _logger.LogWarning($"Invalid daily plan: {dailyPlan}");
                continue;
            }


            workouts.Add(CreateWorkout(
                GetWorkoutTypeFromRunType(runType),
                weekStart.AddDays(i),
                distance,
                description));
        }
        return workouts;
    }
    // Parses a daily plan string into a RunType, distance, and description
    private bool TryParseDailyPlan(string dailyPlan, out RunType runType, out double distance, out string description)
    {
        runType = default;
        distance = default;
        description = null;

        if (string.IsNullOrEmpty(dailyPlan) || dailyPlan.Length < 2)
            return false;

        if (!Enum.TryParse(dailyPlan[..1], out runType))
            return false;

        int distanceEndIndex = dailyPlan.IndexOf("\"") > 0 ? dailyPlan.IndexOf("\"") : dailyPlan.Length;

        if (!double.TryParse(dailyPlan[1..distanceEndIndex], out distance))
            return false;

        if (dailyPlan.Contains("\""))
        {
            int start = dailyPlan.IndexOf("\"") + 1;
            int end = dailyPlan.LastIndexOf("\"");
            description = dailyPlan[start..end];
        }

        return true;
    }
    // Converts a RunType to a WorkoutType
    protected WorkoutType GetWorkoutTypeFromRunType(RunType runType)
    {
        return runType switch
        {
            RunType.E => WorkoutType.EasyRun,
            RunType.R => WorkoutType.RecoveryRun,
            RunType.I => WorkoutType.IntervalTraining,
            RunType.T => WorkoutType.TempoRun,
            RunType.L => WorkoutType.LongRun,
            RunType.M => WorkoutType.MarathonPace,
            RunType.V => WorkoutType.VO2Max,
            _ => throw new ArgumentException($"Invalid RunType: {runType}"),
        };
    }

}

