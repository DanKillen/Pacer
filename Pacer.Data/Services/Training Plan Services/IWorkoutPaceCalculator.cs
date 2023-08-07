using Pacer.Data.Entities;
using Pacer.Data.Extensions;

public interface IWorkoutPaceCalculator
{
    Dictionary<WorkoutType, TimeSpanRange> CalculatePaces(TimeSpan targetTime, RaceType raceType);
}