using Pacer.Data.Entities;
using Pacer.Data.Utilities;

namespace Pacer.Data.Services;

// Calculates maximum and minumum paces for each workout type, done at training plan creation, or when target time is updated
public interface IWorkoutPaceCalculator
{
    public ICollection<TrainingPlanPace> CalculatePaces(TimeSpan targetTime, RaceType raceType, int age, String gender);
}