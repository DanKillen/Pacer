using Pacer.Data.Entities;
using Pacer.Data.Strategies;
using System;

namespace Pacer.Data.Services
{
    public interface IWorkoutFactory
    {
        BaseWorkoutPlanStrategy CreateStrategy(IRunningProfileService runningProfileService, RaceType targetRace, DateTime raceDate, TimeSpan targetTime);
        
        Workout[] AssignWorkouts(IRunningProfileService runningProfileService, RaceType targetRace, DateTime raceDate, TimeSpan targetTime);
    }
}
