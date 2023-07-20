
using Pacer.Data.Entities;
using Pacer.Data.Repositories;
using Pacer.Data.Extensions;
using Pacer.Data.Strategies;

namespace Pacer.Data.Services
{
    public class WorkoutFactory
    {

        public Workout[] AssignWorkouts(RunningProfile runningProfile, RaceType targetRace, DateTime startDate, DateTime endDate, TimeSpan targetTime)
        {
            BaseWorkoutPlanStrategy strategy;

            switch (targetRace)
            {
                case RaceType.Marathon:
                    strategy = new MarathonWorkoutPlanStrategy(runningProfile, startDate, endDate, targetTime);
                    break;
                case RaceType.HalfMarathon:
                    strategy = new HalfMarathonWorkoutPlanStrategy(runningProfile, startDate, endDate, targetTime);
                    break;
                case RaceType.TenK:
                    strategy = new TenKWorkoutPlanStrategy(runningProfile, startDate, endDate, targetTime);
                    break;
                case RaceType.FiveK:
                    strategy = new FiveKWorkoutPlanStrategy(runningProfile, startDate, endDate, targetTime);
                    break;
                default:
                    throw new ArgumentException("Invalid targetRace parameter");
            }

            return strategy.GenerateWorkouts();
        }
    }
        
}
