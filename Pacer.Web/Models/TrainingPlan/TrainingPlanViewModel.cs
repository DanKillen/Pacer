using Pacer.Data.Entities;

public class TrainingPlanViewModel
{
    public int Id { get; set; }
    public RaceType TargetRace { get; set; }
    public string TargetTime { get; set; }
    public string TargetPace { get; set; }
    public IList<WeekViewModel> Weeks { get; set; }
}
public class WeekViewModel
{
    public int WeekNumber { get; set; }
    public IList<WorkoutViewModel> Workouts { get; set; }
}