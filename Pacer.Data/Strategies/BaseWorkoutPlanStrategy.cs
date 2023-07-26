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
            L,   // Long Run
            M  // Race Pace
        }

        protected readonly RunningProfile RunningProfile;
        protected readonly DateTime RaceDate;
        protected readonly TimeSpan TargetTime;
        protected readonly Dictionary<WorkoutType, TimeSpanRange> Paces = new Dictionary<WorkoutType, TimeSpanRange>();

        protected TimeSpan EquivalentMarathonPace { get; }

        protected BaseWorkoutPlanStrategy(RunningProfile runningProfile, DateTime raceDate, TimeSpan targetTime, RaceType raceType)
        {

            RunningProfile = runningProfile;
            RaceDate = raceDate;
            TargetTime = targetTime;

            TimeSpan equivalentMarathonTime = new EquivalentMarathonPaceCalculator().CalculateEquivalentMarathonTime(targetTime, raceType);
            EquivalentMarathonPace = TimeSpan.FromMinutes(equivalentMarathonTime.TotalMinutes / 26.2188);

            Paces.Add(WorkoutType.RecoveryRun, new TimeSpanRange(CalculatePace(1.3), CalculatePace(1.5)));
            Paces.Add(WorkoutType.EasyRun, new TimeSpanRange(CalculatePace(1.15), CalculatePace(1.25)));
            Paces.Add(WorkoutType.LongRun, new TimeSpanRange(CalculatePace(1.1), CalculatePace(1.2)));
            Paces.Add(WorkoutType.IntervalTraining, new TimeSpanRange(CalculatePace(5.0 / 6.0), CalculatePace(14.0 / 15.0)));
            Paces.Add(WorkoutType.TempoRun, new TimeSpanRange(CalculatePace(0.91), CalculatePace(0.94)));
            Paces.Add(WorkoutType.MarathonPace, new TimeSpanRange(CalculatePace(0.99), CalculatePace(1)));
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
            string[] dailyPlans = weekPlan.Split(',');

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

            if (!Enum.TryParse(dailyPlan.Substring(0, 1), out runType))
                return false;

            int distanceEndIndex = dailyPlan.IndexOf("\"") > 0 ? dailyPlan.IndexOf("\"") : dailyPlan.Length;

            if (!double.TryParse(dailyPlan.Substring(1, distanceEndIndex - 1), out distance))
                return false;

            if (dailyPlan.Contains("\""))
            {
                int start = dailyPlan.IndexOf("\"") + 1;
                int end = dailyPlan.LastIndexOf("\"");
                description = dailyPlan.Substring(start, end - start);
            }

            return true;
        }


        protected WorkoutType GetWorkoutTypeFromRunType(RunType runType)
        {
            switch (runType)
            {
                case RunType.E: return WorkoutType.EasyRun;
                case RunType.R: return WorkoutType.RecoveryRun;
                case RunType.I: return WorkoutType.IntervalTraining;
                case RunType.T: return WorkoutType.TempoRun;
                case RunType.L: return WorkoutType.LongRun;
                case RunType.M: return WorkoutType.MarathonPace;
                default: throw new ArgumentException($"Invalid RunType: {runType}");
            }
        }

        private PaceTime CalculatePace(double factor)
        {
            return new PaceTime(TimeSpan.FromMinutes(EquivalentMarathonPace.TotalMinutes * factor));
        }
    }
}
