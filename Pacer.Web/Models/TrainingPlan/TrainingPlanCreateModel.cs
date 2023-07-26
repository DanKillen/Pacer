using System;
using Pacer.Data.Entities;
using System.ComponentModel.DataAnnotations;

public class TrainingPlanCreateModel
{
    [Required]
    public int RunningProfileId { get; set; }

    [Required]
    public RaceType TargetRace { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime RaceDate { get; set; }

    [Required]
    [DataType(DataType.Time)]
    public TimeSpan TargetTime { get; set; }
}
