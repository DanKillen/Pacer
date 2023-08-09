
using Pacer.Data.Entities;
using Pacer.Data.Services;
using Pacer.Data.Extensions;
using Microsoft.Extensions.Logging;

namespace Pacer.Data.Strategies
{
    public class BeginnerMarathonWorkoutPlanStrategy : BaseWorkoutPlanStrategy
    {
        private readonly string[] WeekPlans = {
            // base phase
            "L7;X;E6;R3;E5;X;R5",
            "L8;X;E8;E9;X;E5;R4",
            "L12;X;R5;X;E9;X;R5",
            // build phase
            "M13\"Run the first 8 miles at a comfortable pace and then the last 5 miles at target pace\";X;T10\"Run the first 5 miles at a comfortable pace and then the last 5 miles at target pace\";X;E6;X;R4",
            "L18;X;E6;T8\"Run the first 3 miles at a comfortable pace and then the last 5 miles at target pace\";X;E8",
            "L14;X;I5\"5 miles at target pace but take 60 second walking breaks in between each mile\";R5;X;E5;R4",
            "L20;X;R7;T10\"Run the first 3 miles at a comfortable pace and then the last 7 miles at target pace\";X;E8;R5",
            // peak phase
            "M18\"Run the first 8 miles at a comfortable pace and then the last 10 at target pace\";X;E8;I8\"8 miles at target pace but take 60 second walking breaks in between each mile\";X;L13;R5",
            "L16;X;E8;L11;X;R4;T6",
            "L17;X;R7;I7\"7 miles at target pace but take 60 second walking breaks in between each mile\";X;L11;R4",
            // taper phase
            "L20;X;E8;R6;X;R4;I5\"8 miles at target pace but take 60 second walking breaks in between each mile\"",
            "L20;X;E7;T5;X;R5;X",
            "L12;X;R6;M7\"Run the first 5 miles at a comfortable pace and then the last 2 miles at target pace\";X;R5;R4;X",
        };
        
        public BeginnerMarathonWorkoutPlanStrategy(IRunningProfileService runningProfileService, IWorkoutPaceCalculator workoutPaceCalculator, DateTime raceDate, TimeSpan targetTime)
        : base(runningProfileService, workoutPaceCalculator, raceDate, targetTime) // Pass the dependencies to the base class constructor
    {
    }

        public override Workout[] GenerateWorkouts()
        {
            var workouts = new List<Workout>();
            DateTime currentWeekStart = RaceDate.AddDays(-7 * WeekPlans.Length);

            for (int week = 0; week < WeekPlans.Length; week++)
            {
                try
                {
                    workouts.AddRange(CreateWorkoutsForWeek(WeekPlans[week], currentWeekStart));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred during the parsing of week {week + 1}: {ex.Message}");
                    // If you wish to stop the execution when an error occurs, uncomment the line below
                    // throw;
                }

                currentWeekStart = currentWeekStart.AddDays(7);
            }

            return workouts.ToArray();
        }

    }
}
