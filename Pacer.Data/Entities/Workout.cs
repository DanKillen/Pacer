using System;
using Pacer.Data.Extensions;

namespace Pacer.Data.Entities
{
    public enum WorkoutType
    {
        RecoveryRun,
        EasyRun,
        LongRun,
        IntervalTraining,
        LactateThreshold,
        Rest
    }

    public class TimeSpanRange
    {
        public PaceTimeSpan Min { get; set; }
        public PaceTimeSpan Max { get; set; }

        public TimeSpanRange(PaceTimeSpan min, PaceTimeSpan max)
        {
            Min = min;
            Max = max;
        }

        public override string ToString()
        {
            return $"{Min} - {Max}";
        }
    }

    public class Workout
    {
        public int Id { get; set; }

        // TrainingPlan entity reference - to link a workout to a training plan
        public TrainingPlan TrainingPlan { get; set; }
        public int TrainingPlanId { get; set; }

        // Workout specific properties
        public WorkoutType Type { get; set; }
        public DateTime Date { get; set; }
        public double TargetDistance { get; set; }
        public TimeSpanRange TargetPace { get; set; }
        public string WorkoutDescription { get; set; }
    }
}