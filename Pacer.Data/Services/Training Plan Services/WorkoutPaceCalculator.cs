using Pacer.Data.Entities;
using Pacer.Data.Extensions;

public class WorkoutPaceCalculator : IWorkoutPaceCalculator
{
    private readonly EquivalentMarathonPaceCalculator _equivalentMarathonPaceCalculator;

    public WorkoutPaceCalculator(EquivalentMarathonPaceCalculator equivalentMarathonPaceCalculator)
    {
        _equivalentMarathonPaceCalculator = equivalentMarathonPaceCalculator;
    } 

    public Dictionary<WorkoutType, TimeSpanRange> CalculatePaces(TimeSpan targetTime, RaceType raceType)
    {
        TimeSpan equivalentMarathonPace = _equivalentMarathonPaceCalculator.CalculateEquivalentMarathonPace(targetTime, raceType);

        var paces = new Dictionary<WorkoutType, TimeSpanRange>
        {
            { WorkoutType.RecoveryRun, new TimeSpanRange(CalculatePace(1.3, equivalentMarathonPace), CalculatePace(1.5, equivalentMarathonPace)) },
            { WorkoutType.EasyRun, new TimeSpanRange(CalculatePace(1.15, equivalentMarathonPace), CalculatePace(1.25, equivalentMarathonPace)) },
            { WorkoutType.LongRun, new TimeSpanRange(CalculatePace(1.1, equivalentMarathonPace), CalculatePace(1.2, equivalentMarathonPace)) },
            { WorkoutType.VO2Max, new TimeSpanRange(CalculatePace(0.85, equivalentMarathonPace), CalculatePace(0.9, equivalentMarathonPace)) },
            { WorkoutType.IntervalTraining, new TimeSpanRange(CalculatePace(5.0 / 6.0, equivalentMarathonPace), CalculatePace(14.0 / 15.0, equivalentMarathonPace)) },
            { WorkoutType.TempoRun, new TimeSpanRange(CalculatePace(0.91, equivalentMarathonPace), CalculatePace(0.94, equivalentMarathonPace)) },
            { WorkoutType.MarathonPace, new TimeSpanRange(CalculatePace(0.99, equivalentMarathonPace), CalculatePace(1, equivalentMarathonPace)) }
        };

        return paces;
    }

    private PaceTime CalculatePace(double factor, TimeSpan equivalentMarathonPace)
    {
        return new PaceTime(TimeSpan.FromMinutes(equivalentMarathonPace.TotalMinutes * factor));
    }
}