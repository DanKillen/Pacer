using Pacer.Data.Entities;
using Pacer.Data.Repositories;
using Pacer.Data.Utilities;
using Pacer.Data.Strategies;
using Microsoft.Extensions.Logging;

namespace Pacer.Data.Services
{
    public class WorkoutFactory : IWorkoutFactory
    {

        public BasePlanStrategy CreateStrategy(RaceType targetRace, DateTime raceDate, TimeSpan targetTime)
        {
            return targetRace switch
            {
                RaceType.Marathon => new MarathonPlanStrategy(raceDate, targetTime),
                RaceType.BMarathon => new BeginnerMarathonPlanStrategy(raceDate, targetTime),
                RaceType.HalfMarathon => new HalfMarathonPlanStrategy(raceDate, targetTime),
                RaceType.BHalfMarathon => new BeginnerHalfMarathonPlanStrategy(raceDate, targetTime),
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
