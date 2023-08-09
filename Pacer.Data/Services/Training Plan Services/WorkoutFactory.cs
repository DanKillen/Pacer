using Pacer.Data.Entities;
using Pacer.Data.Repositories;
using Pacer.Data.Utilities;
using Pacer.Data.Strategies;
using Microsoft.Extensions.Logging;

namespace Pacer.Data.Services
{
    public class WorkoutFactory : IWorkoutFactory
    {

        public BaseWorkoutPlanStrategy CreateStrategy(RaceType targetRace, DateTime raceDate, TimeSpan targetTime)
        {
            return targetRace switch
            {
                RaceType.Marathon => new AdvancedMarathonWorkoutPlanStrategy(raceDate, targetTime),
                RaceType.BMarathon => new BeginnerMarathonWorkoutPlanStrategy(raceDate, targetTime),
                RaceType.HalfMarathon => new AdvancedHalfMarathonWorkoutPlanStrategy(raceDate, targetTime),
                RaceType.BHalfMarathon => new BeginnerHalfMarathonWorkoutPlanStrategy(raceDate, targetTime),
                _ => throw new ArgumentException("Invalid targetRace parameter"),
            };
        }

        public Workout[] AssignWorkouts(RaceType targetRace, DateTime raceDate, TimeSpan targetTime)
        {
            var strategy = CreateStrategy(targetRace, raceDate, targetTime);
            return strategy.GenerateWorkouts();
        }
    }

}
