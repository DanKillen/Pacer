
using Pacer.Data.Entities;
using Pacer.Data.Repositories;
using Pacer.Data.Extensions;
using Pacer.Data.Strategies;
using Microsoft.Extensions.Logging;

namespace Pacer.Data.Services
{
    public class WorkoutFactory
    {
        private readonly IWorkoutPaceCalculator _workoutPaceCalculator;

        public WorkoutFactory(IWorkoutPaceCalculator workoutPaceCalculator)
        {
            _workoutPaceCalculator = workoutPaceCalculator;
        }

        public BaseWorkoutPlanStrategy CreateStrategy(RunningProfile runningProfile, RaceType targetRace, DateTime raceDate, TimeSpan targetTime)
        {
            return targetRace switch
            {
                RaceType.Marathon => new MarathonWorkoutPlanStrategy(runningProfile, raceDate, targetTime, _workoutPaceCalculator),
                RaceType.HalfMarathon => new HalfMarathonWorkoutPlanStrategy(runningProfile, raceDate, targetTime, _workoutPaceCalculator),
                RaceType.TenK => new TenKWorkoutPlanStrategy(runningProfile, raceDate, targetTime, _workoutPaceCalculator),
                RaceType.FiveK => new FiveKWorkoutPlanStrategy(runningProfile, raceDate, targetTime, _workoutPaceCalculator),
                _ => throw new ArgumentException("Invalid targetRace parameter"),
            };
        }

        public Workout[] AssignWorkouts(RunningProfile runningProfile, RaceType targetRace, DateTime raceDate, TimeSpan targetTime)
        {
            var strategy = CreateStrategy(runningProfile, targetRace, raceDate, targetTime);
            return strategy.GenerateWorkouts();
        }
    }

}
