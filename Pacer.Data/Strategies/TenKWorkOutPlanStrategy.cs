
using Pacer.Data.Entities;
using Pacer.Data.Services;
using Pacer.Data.Extensions;
using Microsoft.Extensions.Logging;

namespace Pacer.Data.Strategies
{
    public class TenKWorkoutPlanStrategy : BaseWorkoutPlanStrategy
    {
        private readonly string[] weekPlans = {
            // Preparatory Phase
            "X,E3,R2,X,E3,R2,L4",
            "X,E4,R2,X,E4,R2,L5",
            "X,E5,T4\"Run the first 2 miles at a comfortable pace and then the last 2 at $pace\",X,E5,R3,L6",
            // Build Phase
            "X,E6,R3,X,E6,T5\"Run the first 2 miles at a comfortable pace and then the last 3 at $pace\",L7",
            "X,E7,R3,X,E7,V3\"Aim for a fast, nearly all-out pace\",L8",
            "X,E7,R4,X,E7,T6\"Run the first 2 miles at a comfortable pace and then the last 4 at $pace\",L9",
            // Peak Phase
            "X,E8,R4,X,E8,V4\"Aim for a fast, nearly all-out pace\",L10",
            // Taper Phase
            "X,E6,R3,X,E5,R2,L7",
            "X,E5,R2,X,E4,R2,X",
        };
        
        public TenKWorkoutPlanStrategy(RunningProfile runningProfile, DateTime raceDate, TimeSpan targetTime)
            : base(runningProfile, raceDate, targetTime, RaceType.TenK)
        {
        }

        public override Workout[] GenerateWorkouts()
        {
            var workouts = new List<Workout>();
            DateTime currentWeekStart = RaceDate.AddDays(-7 * weekPlans.Length);

            for (int week = 0; week < weekPlans.Length; week++)
            {
                try
                {
                    workouts.AddRange(CreateWorkoutsForWeek(weekPlans[week], currentWeekStart));
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
