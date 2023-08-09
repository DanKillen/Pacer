using Pacer.Data.Entities;
using Pacer.Web;

public class TrainingPlanCalendarViewModel
{
    public int Id { get; set; }
    public RaceType TargetRace { get; set; }
    public string TargetRaceDisplayName => TargetRace.GetDisplayName();
    public string TargetTime { get; set; }
    public string TargetPace { get; set; }
    public DateTime RaceDate { get; set; }    
    
    public int Month  { get; set; }
    public int Year { get; set; }
    public List<WorkoutViewModel> Workouts { get; set; }

}
