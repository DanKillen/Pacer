using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
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
    public string WorkoutDescription { get; set; }
    public double ActualDistance { get; set; }
    public TimeSpan ActualTime { get; set; }
}
