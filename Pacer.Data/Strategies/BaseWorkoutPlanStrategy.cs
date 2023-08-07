using Microsoft.Extensions.Logging;
using Pacer.Data.Entities;
using Pacer.Data.Extensions;


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

        protected TimeSpan EquivalentMarathonPace { get; private set; }
        protected IWorkoutPaceCalculator WorkoutPaceCalculator { get; }
        protected IDictionary<WorkoutType, TimeSpanRange> Paces { get; private set; }

        protected BaseWorkoutPlanStrategy(RunningProfile runningProfile, DateTime raceDate, TimeSpan targetTime, IWorkoutPaceCalculator workoutPaceCalculator)
        {

            RunningProfile = runningProfile;
            RaceDate = raceDate;
            TargetTime = targetTime;
            WorkoutPaceCalculator = workoutPaceCalculator;
        }
        protected void InitializePaces(RaceType raceType)
        {
            Paces = WorkoutPaceCalculator.CalculatePaces(TargetTime, raceType);
        }
        public abstract Workout[] GenerateWorkouts();

        protected Workout CreateWorkout(WorkoutType type, DateTime date, double targetDistance, string description = null)
        {
            var targetPace = Paces[type];

            string finalDescription;

            if (description != null)
            {
                finalDescription = description.Replace("$pace", $"{targetPace.Min} to {targetPace.Max} per mile");
            }
            else
            {
                finalDescription = $"Run {targetDistance} miles at a pace of {targetPace.Min} to {targetPace.Max} per mile";
            }
            return new Workout
            {
                Type = type,
                Date = date,
                TargetDistance = targetDistance,
                TargetPaceMinMinutes = targetPace.Min.Minutes,
                TargetPaceMinSeconds = targetPace.Min.Seconds,
                TargetPaceMaxMinutes = targetPace.Max.Minutes,
                TargetPaceMaxSeconds = targetPace.Max.Seconds,
                WorkoutDescription = finalDescription
            };
        }

        protected IEnumerable<Workout> CreateWorkoutsForWeek(string weekPlan, DateTime weekStart)
        {
            var workouts = new List<Workout>();
            string[] dailyPlans = weekPlan.Split(';');

            for (int i = 0; i < dailyPlans.Length; i++)
            {
                string dailyPlan = dailyPlans[i].Trim();

                if (!TryParseDailyPlan(dailyPlan, out RunType runType, out double distance, out string description))
                {
                    Console.WriteLine($"Invalid daily plan: {dailyPlan}");
                    continue;
                }

                if (runType == RunType.X)
                {
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
