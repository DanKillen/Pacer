using Pacer.Data.Entities;
using Pacer.Data.Utilities;

namespace Pacer.Data.Services;

public class WorkoutPaceCalculator : IWorkoutPaceCalculator
{
    private readonly IRaceTimePredictor _raceTimePredictor;

    public WorkoutPaceCalculator(IRaceTimePredictor raceTimePredictor)
    {
        _raceTimePredictor = raceTimePredictor ?? throw new ArgumentNullException(nameof(raceTimePredictor));
    }
    // Calculates maximum and minumum paces for each workout type, done at training plan creation, or when target time is updated
    public ICollection<TrainingPlanPace> CalculatePaces(TimeSpan targetTime, RaceType raceType, int age, string gender)
    {

        TimeSpan equivalentMarathonPace = _raceTimePredictor.CalculateEquivalentMarathonPace(targetTime, raceType, age, gender);


        var paces = new List<TrainingPlanPace>();

        foreach (WorkoutType type in Enum.GetValues(typeof(WorkoutType)))
        {
            var paceValues = CalculatePace(type, equivalentMarathonPace);

            paces.Add(new TrainingPlanPace
            {
                WorkoutType = type,
                PaceType = PaceType.Min,
                Pace = paceValues.Item1
            });

            paces.Add(new TrainingPlanPace
            {
                WorkoutType = type,
                PaceType = PaceType.Max,
                Pace = paceValues.Item2
            });
        }

        return paces;
    }

    // Calculates the pace range for a given workout type
    private Tuple<PaceTime, PaceTime> CalculatePace(WorkoutType type, TimeSpan equivalentMarathonPace)
    {
        double minMultiplier, maxMultiplier;

        switch (type)
        {
            case WorkoutType.RecoveryRun:
                minMultiplier = 1.3;
                maxMultiplier = 1.5;
                break;
            case WorkoutType.EasyRun:
                minMultiplier = 1.15;
                maxMultiplier = 1.25;
                break;
            case WorkoutType.LongRun:
                minMultiplier = 1.1;
                maxMultiplier = 1.2;
                break;
            case WorkoutType.VO2Max:
                minMultiplier = 0.85;
                maxMultiplier = 0.9;
                break;
            case WorkoutType.IntervalTraining:
                minMultiplier = 5.0 / 6.0;
                maxMultiplier = 14.0 / 15.0;
                break;
            case WorkoutType.TempoRun:
                minMultiplier = 0.91;
                maxMultiplier = 0.94;
                break;
            case WorkoutType.MarathonPace:
                minMultiplier = 0.99;
                maxMultiplier = 1;
                break;
            default:
                throw new ArgumentException("Unsupported workout type", nameof(type));
        }

        var minPace = TimeSpan.FromTicks((long)(equivalentMarathonPace.Ticks * minMultiplier));
        var maxPace = TimeSpan.FromTicks((long)(equivalentMarathonPace.Ticks * maxMultiplier));

        return Tuple.Create(ConvertToPaceTime(minPace), ConvertToPaceTime(maxPace));
    }
    
    // Converts a TimeSpan to a PaceTime
    private PaceTime ConvertToPaceTime(TimeSpan timeSpan)
    {
        return new PaceTime(timeSpan);
    }
}