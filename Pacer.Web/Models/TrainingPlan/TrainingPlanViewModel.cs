using Pacer.Data.Entities;

public class TrainingPlanViewModel
{
    public int Id { get; set; }
    public string PlanName { get; set; }
    public IList<WeekViewModel> Weeks { get; set; }
}

public class WeekViewModel
{
    public int WeekNumber { get; set; }
    public IList<WorkoutViewModel> Workouts { get; set; }
}

public class WorkoutViewModel
{
    public int Id { get; set; }
    public WorkoutType Type { get; set; }
    public DateTime Date { get; set; }
    public double TargetDistance { get; set; }
    public int TargetPaceMinMinutes { get; set; }
    public int TargetPaceMinSeconds { get; set; }
    public int TargetPaceMaxMinutes { get; set; }
    public int TargetPaceMaxSeconds { get; set; }
    public string WorkoutDescription { get; set; }
}
