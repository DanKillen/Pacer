using Pacer.Data.Entities;
using Pacer.Data.Extensions;

public interface IWorkoutPaceCalculator
{
    public ICollection<TrainingPlanPace> CalculatePaces(TimeSpan targetTime, RaceType raceType);
    
}