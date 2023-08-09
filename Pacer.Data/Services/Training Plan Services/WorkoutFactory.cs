using Pacer.Data.Entities;
using Pacer.Data.Repositories;
using Pacer.Data.Extensions;
using Pacer.Data.Strategies;
using Microsoft.Extensions.Logging;

namespace Pacer.Data.Services
{
    public class WorkoutFactory : IWorkoutFactory
    {
        private readonly IRunningProfileService _runningProfileService;
        private readonly IWorkoutPaceCalculator _workoutPaceCalculator;
    
        public WorkoutFactory(IRunningProfileService runningProfileService,IWorkoutPaceCalculator workoutPaceCalculator)
        {
            _runningProfileService = runningProfileService;
            _workoutPaceCalculator = workoutPaceCalculator;
        }

        public BaseWorkoutPlanStrategy CreateStrategy(IRunningProfileService _runningProfileService, RaceType targetRace, DateTime raceDate, TimeSpan targetTime)
        {
            return targetRace switch
            {
                RaceType.Marathon => new AdvancedMarathonWorkoutPlanStrategy(_runningProfileService, _workoutPaceCalculator, raceDate, targetTime),
                RaceType.BMarathon => new BeginnerMarathonWorkoutPlanStrategy(_runningProfileService, _workoutPaceCalculator, raceDate, targetTime),
                RaceType.HalfMarathon => new AdvancedHalfMarathonWorkoutPlanStrategy(_runningProfileService, _workoutPaceCalculator, raceDate, targetTime),
                RaceType.BHalfMarathon => new BeginnerHalfMarathonWorkoutPlanStrategy(_runningProfileService, _workoutPaceCalculator, raceDate, targetTime),
                _ => throw new ArgumentException("Invalid targetRace parameter"),
            };
        }

        public Workout[] AssignWorkouts(IRunningProfileService runningProfileService, RaceType targetRace, DateTime raceDate, TimeSpan targetTime)
        {
            var strategy = CreateStrategy(runningProfileService, targetRace, raceDate, targetTime);
            return strategy.GenerateWorkouts();
        }
    }

}
