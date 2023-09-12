using Pacer.Data.Entities;
using Pacer.Data.Strategies;
using System;

namespace Pacer.Data.Services;
public interface IWorkoutFactory
{
    // Pulls the correct Training Plan based on selected race
    //BasePlanStrategy CreateStrategy(RaceType targetRace, DateTime raceDate, TimeSpan targetTime);

    // Generates the workouts for the selected training plan
    Workout[] AssignWorkouts(RaceType targetRace, DateTime raceDate, TimeSpan targetTime);
}
