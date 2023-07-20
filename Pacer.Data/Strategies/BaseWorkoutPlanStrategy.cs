using Pacer.Data.Entities;
using Pacer.Data.Extensions;

namespace Pacer.Data.Strategies
{
    public abstract class BaseWorkoutPlanStrategy
    {
        protected readonly RunningProfile RunningProfile;
        protected readonly DateTime StartDate;
        protected readonly DateTime EndDate;
        protected readonly TimeSpan TargetTime;

        protected TimeSpanRange RecoveryRunPace { get; set; }
        protected TimeSpanRange EasyRunPace { get; set; }
        protected TimeSpanRange LongRunPace { get; set; }
        protected TimeSpanRange IntervalTrainingPace { get; set; }
        protected TimeSpanRange LactateThresholdPace { get; set; }

        protected TimeSpan EquivalentMarathonPace { get; }
        protected EquivalentMarathonPaceCalculator PaceCalculator { get; }

        protected BaseWorkoutPlanStrategy(RunningProfile runningProfile, DateTime startDate, DateTime endDate, TimeSpan targetTime, RaceType raceType)
        {
            RunningProfile = runningProfile;
            StartDate = startDate;
            EndDate = endDate;
            TargetTime = targetTime;

            EquivalentMarathonPaceCalculator paceCalculator = new EquivalentMarathonPaceCalculator();
            TimeSpan equivalentMarathonTime = paceCalculator.CalculateEquivalentMarathonTime(targetTime, raceType);
            double targetPaceMinutes = equivalentMarathonTime.TotalMinutes / 26.2188;
            EquivalentMarathonPace = TimeSpan.FromMinutes(targetPaceMinutes);

            RecoveryRunPace = new TimeSpanRange(CalculateRecoveryRunMinPace(), CalculateRecoveryRunMaxPace());
            EasyRunPace = new TimeSpanRange(CalculateEasyRunMinPace(), CalculateEasyRunMaxPace());
            LongRunPace = new TimeSpanRange(CalculateLongRunMinPace(), CalculateLongRunMaxPace());
            IntervalTrainingPace = new TimeSpanRange(CalculateIntervalTrainingMinPace(), CalculateIntervalTrainingMaxPace());
            LactateThresholdPace = new TimeSpanRange(CalculateLactateThresholdMinPace(), CalculateLactateThresholdMaxPace());
        }
        public abstract Workout[] GenerateWorkouts();

        protected Workout CreateWorkout(WorkoutType type, DateTime date, double targetDistance, TimeSpanRange targetPace, string description = null)
        {
            return new Workout
            {
                Type = type,
                Date = date,
                TargetDistance = targetDistance,
                TargetPace = targetPace,
                WorkoutDescription = description ?? $"{targetDistance} mile {type} at a pace of {targetPace.Min} to {targetPace.Max}"
            };
        }

        protected PaceTimeSpan CalculateRecoveryRunMinPace()
        {
            return new PaceTimeSpan(TimeSpan.FromMinutes(EquivalentMarathonPace.TotalMinutes * 1.3));
        }
        protected PaceTimeSpan CalculateRecoveryRunMaxPace()
        {
            return new PaceTimeSpan(TimeSpan.FromMinutes(EquivalentMarathonPace.TotalMinutes * 1.5));
        }
        protected PaceTimeSpan CalculateEasyRunMinPace()
        {
            return new PaceTimeSpan(TimeSpan.FromMinutes(EquivalentMarathonPace.TotalMinutes * 1.15));
        }

        protected PaceTimeSpan CalculateEasyRunMaxPace()
        {
            return new PaceTimeSpan(TimeSpan.FromMinutes(EquivalentMarathonPace.TotalMinutes * 1.25));
        }

        protected PaceTimeSpan CalculateLongRunMinPace()
        {
            return new PaceTimeSpan(TimeSpan.FromMinutes(EquivalentMarathonPace.TotalMinutes * 1.1));
        }

        protected PaceTimeSpan CalculateLongRunMaxPace()
        {
            return new PaceTimeSpan(TimeSpan.FromMinutes(EquivalentMarathonPace.TotalMinutes * 1.2));
        }

        protected PaceTimeSpan CalculateIntervalTrainingMinPace()
        {
            return new PaceTimeSpan(TimeSpan.FromMinutes(EquivalentMarathonPace.TotalMinutes * (5.0 / 6.0)));
        }

        protected PaceTimeSpan CalculateIntervalTrainingMaxPace()
        {
            return new PaceTimeSpan(TimeSpan.FromMinutes(EquivalentMarathonPace.TotalMinutes * (14.0 / 15.0)));
        }

        protected PaceTimeSpan CalculateLactateThresholdMinPace()
        {
            return new PaceTimeSpan(TimeSpan.FromMinutes(EquivalentMarathonPace.TotalMinutes * 0.91));
        }

        protected PaceTimeSpan CalculateLactateThresholdMaxPace()
        {
            return new PaceTimeSpan(TimeSpan.FromMinutes(EquivalentMarathonPace.TotalMinutes * 0.94));
        }
    }
}
