
using Pacer.Data.Entities;
using Pacer.Data.Repositories;
using Pacer.Data.Extensions;
using Pacer.Data.Strategies;
using Microsoft.Extensions.Logging;

namespace Pacer.Data.Services
{
    public class WorkoutFactory
    {

        public Workout[] AssignWorkouts(RunningProfile runningProfile, RaceType targetRace, DateTime raceDate, TimeSpan targetTime)
        {
            BaseWorkoutPlanStrategy strategy;

            switch (targetRace)
            {
                case RaceType.Marathon:
                    strategy = new MarathonWorkoutPlanStrategy(runningProfile, raceDate, targetTime);
                    break;
                case RaceType.HalfMarathon:
                    strategy = new HalfMarathonWorkoutPlanStrategy(runningProfile, raceDate, targetTime);
                    break;
                case RaceType.TenK:
                    strategy = new TenKWorkoutPlanStrategy(runningProfile, raceDate, targetTime);
                    break;
                case RaceType.FiveK:
                    strategy = new FiveKWorkoutPlanStrategy(runningProfile, raceDate, targetTime);
                    break;
                default:
                    throw new ArgumentException("Invalid targetRace parameter");
            }

            return strategy.GenerateWorkouts();
        }
    }
        
}
