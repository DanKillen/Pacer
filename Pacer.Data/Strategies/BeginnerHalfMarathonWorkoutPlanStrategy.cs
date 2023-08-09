
using Pacer.Data.Entities;
using Pacer.Data.Services;
using Pacer.Data.Extensions;

namespace Pacer.Data.Strategies
{
    public class BeginnerHalfMarathonWorkoutPlanStrategy : BaseWorkoutPlanStrategy
    {

        private readonly string[] WeekPlans = {
            // Preparatory Phase
            "X;E3;R2;X;E3;R2;L4",
            "X;E4;R2;X;E4;R2;L5",
            "X;E5;T4\"Run the first 2 miles at a comfortable pace and then the last 2 at target pace\";X;E5;R3;L6",
            // Build Phase
            "X;E6;R3;X;E6;T5\"Run the first 2 miles at a comfortable pace and then the last 3 at target pace\";L7",
            "X;E7;R3;X;E7;T5\"Run the first 2 miles at a comfortable pace and then the last 3 at target pace\";L8",
            "X;E7;R4;X;E7;T6\"Run the first 2 miles at a comfortable pace and then the last 4 at target pace\";L9",
            // Peak Phase
            "X;E8;R4;X;E8;T6\"Run the first 2 miles at a comfortable pace and then the last 4 at target pace\";L10",
            "X;E9;R4;X;E9;T7\"Run the first 2 miles at a comfortable pace and then the last 5 at target pace\";L11",
            // Taper Phase
            "X;E6;R3;X;E5;R2;L8",
            "X;E5;R2;X;E4;R2;X",
        };
        public BeginnerHalfMarathonWorkoutPlanStrategy(IRunningProfileService runningProfileService, IWorkoutPaceCalculator workoutPaceCalculator, DateTime raceDate, TimeSpan targetTime)
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
                }
                currentWeekStart = currentWeekStart.AddDays(7);
            }

            return workouts.ToArray();
        }

    }
}
