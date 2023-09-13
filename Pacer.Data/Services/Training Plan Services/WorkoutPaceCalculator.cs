using Pacer.Data.Entities;
using Pacer.Data.Utilities;

namespace Pacer.Data.Services;

public class WorkoutPaceCalculator : IWorkoutPaceCalculator
{
    const double RecoveryRunMin = 1.3;
    const double RecoveryRunMax = 1.5;
    const double EasyRunMin = 1.15;
    const double EasyRunMax = 1.25;
    const double LongRunMin = 1.1;
    const double LongRunMax = 1.2;
    const double VO2MaxMin = 0.85;
    const double VO2MaxMax = 0.9;
    const double IntervalTrainingMin = 5.0 / 6.0;
    const double IntervalTrainingMax = 14.0 / 15.0;
    const double TempoRunMin = 0.91;
    const double TempoRunMax = 0.94;
    const double MarathonPaceMin = 0.99;
    const double MarathonPaceMax = 1;

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
                minMultiplier = RecoveryRunMin;
                maxMultiplier = RecoveryRunMax;
                break;
            case WorkoutType.EasyRun:
                minMultiplier = EasyRunMin;
                maxMultiplier = EasyRunMax;
                break;
            case WorkoutType.LongRun:
                minMultiplier = LongRunMin;
                maxMultiplier = LongRunMax;
                break;
            case WorkoutType.VO2Max:
                minMultiplier = VO2MaxMin;
                maxMultiplier = VO2MaxMax;
                break;
            case WorkoutType.IntervalTraining:
                minMultiplier = IntervalTrainingMin;
                maxMultiplier = IntervalTrainingMax;
                break;
            case WorkoutType.TempoRun:
                minMultiplier = TempoRunMin;
                maxMultiplier = TempoRunMax;
                break;
            case WorkoutType.MarathonPace:
                minMultiplier = MarathonPaceMin;
                maxMultiplier = MarathonPaceMax;
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