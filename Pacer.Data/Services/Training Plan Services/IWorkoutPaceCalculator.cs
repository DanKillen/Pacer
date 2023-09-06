using Pacer.Data.Entities;
using Pacer.Data.Utilities;

public interface IWorkoutPaceCalculator
{
    public ICollection<TrainingPlanPace> CalculatePaces(TimeSpan targetTime, RaceType raceType, int age, String gender);
}