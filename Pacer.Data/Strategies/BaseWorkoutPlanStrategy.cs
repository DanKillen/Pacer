using Microsoft.Extensions.Logging;
using Pacer.Data.Entities;
using Pacer.Data.Utilities;
using Pacer.Data.Services;


namespace Pacer.Data.Strategies
{
    public abstract class BaseWorkoutPlanStrategy
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

        protected readonly RunningProfile RunningProfile;
        protected readonly DateTime RaceDate;
        protected readonly TimeSpan TargetTime;


        protected BaseWorkoutPlanStrategy(DateTime raceDate, TimeSpan targetTime)
        {
            RaceDate = raceDate;
            TargetTime = targetTime;
        }

        public abstract Workout[] GenerateWorkouts();
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
                    Console.WriteLine($"Invalid daily plan: {dailyPlan}");
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
}
