using System;
using Pacer.Data.Entities;
using System.ComponentModel.DataAnnotations;

public class TrainingPlanCreateModel
{
    [Required]
    public int RunningProfileId { get; set; }

    [Required]
    public RaceType TargetRace { get; set; }
    
    public string[] Recommendation { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime RaceDate { get; set; }

    [Required]
    [DataType(DataType.Time)]
    public TimeSpan TargetTime { get; set; }
    
    public TimeSpan EstimatedMarathonTime { get; set; }
    
    public TimeSpan EstimatedHalfMarathonTime { get; set; }
}
