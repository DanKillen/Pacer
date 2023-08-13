using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pacer.Data.Utilities;

namespace Pacer.Data.Entities;
public enum WorkoutType
{
    [Display(Name = "Recovery Run")]
    RecoveryRun,
    [Display(Name = "Easy Run")]
    EasyRun,
    [Display(Name = "Long Run")]
    LongRun,
    [Display(Name = "Race Pace")]
    MarathonPace,
    [Display(Name = "Interval Training")]
    IntervalTraining,
    [Display(Name = "Tempo Run")]
    TempoRun,
    [Display(Name = "VO2 Max Run")]
    VO2Max,
}

public class TimeSpanRange
{
    public int MinMinutes { get; set; }
    public int MinSeconds { get; set; }
    public int MaxMinutes { get; set; }
    public int MaxSeconds { get; set; }
    [NotMapped]
    public PaceTime Min
    {
        get { return new PaceTime(new TimeSpan(0, MinMinutes, MinSeconds)); }
        set
        {
            MinMinutes = value.Minutes;
            MinSeconds = value.Seconds;
        }
    }

    [NotMapped]
    public PaceTime Max
    {
        get { return new PaceTime(new TimeSpan(0, MaxMinutes, MaxSeconds)); }
        set
        {
            MaxMinutes = value.Minutes;
            MaxSeconds = value.Seconds;
        }
    }

    public TimeSpanRange() { }

    public TimeSpanRange(PaceTime min, PaceTime max)
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
    [NotMapped]
    public string FormattedDate => Date.ToString("yyyy-MM-dd");
    public double TargetDistance { get; set; }
    public string WorkoutDescription { get; set; }
    public double ActualDistance { get; set; }
    public TimeSpan ActualTime { get; set; }
}
