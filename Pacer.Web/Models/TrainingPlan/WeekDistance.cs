namespace Pacer.Web.Models.TrainingPlan;
public class WeekDistance
{
    public int Week { get; set; }
    public double TargetDistance { get; set; }
    public double ActualDistance { get; set; }
    public double PercentageComplete 
    { 
        get 
        {
            return TargetDistance == 0 ? 0 : (ActualDistance / TargetDistance) * 100;
        }
    }
}
