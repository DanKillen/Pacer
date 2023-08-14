using Pacer.Data.Entities;
using Pacer.Web;
using Pacer.Web.Models.TrainingPlan;

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
    public List<WeekDistance> WeekDistances { get; set; }
    public List<double> TargetDistances => WeekDistances.Select(w => w.TargetDistance).ToList();
    public List<double> ActualDistances => WeekDistances.Select(w => w.ActualDistance).ToList();

}
