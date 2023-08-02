using Pacer.Data.Entities;

public class TrainingPlanCalendarViewModel
{
    public int Id { get; set; }
    public RaceType TargetRace { get; set; }
    public string TargetTime { get; set; }
    public string TargetPace { get; set; }
    public DateTime RaceDate { get; set; }    
    
    public int Month  { get; set; }
    public int Year { get; set; }
    public List<WorkoutViewModel> Workouts { get; set; }

}
