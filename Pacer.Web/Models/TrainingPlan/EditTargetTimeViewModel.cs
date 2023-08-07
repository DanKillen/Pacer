using Pacer.Data.Entities;

public class EditTargetTimeViewModel
{
    public int TrainingPlanId { get; set; }
    public RaceType TargetRace { get; set; }
    public TimeSpan TargetTime { get; set; }
}
