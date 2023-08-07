using System;
using Pacer.Data.Entities;
using System.ComponentModel.DataAnnotations;

public class TrainingPlanDeleteModel
{
    [Required]
    public int Id { get; set; }

    [Required]
    public RaceType TargetRace { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime RaceDate { get; set; }

    [Required]
    [DataType(DataType.Time)]
    public TimeSpan TargetTime { get; set; }
}
