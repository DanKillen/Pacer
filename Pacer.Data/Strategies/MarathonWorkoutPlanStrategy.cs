using Pacer.Data.Entities;
using Pacer.Data.Services;
using Pacer.Data.Utilities;

namespace Pacer.Data.Strategies
{
    public class MarathonWorkoutPlanStrategy : BaseWorkoutPlanStrategy
    {

        private readonly string[] WeekPlans = {
            // base phase
            "L6;X;E6;R3;E5;X;R5",
            "L7;X;E6;R3;E5;X;R5",
            "L8;X;E6;R3;E5;X;R5",
            "L8;X;E8;E9;X;E5;R4",
            // build phase
            "L12;X;R5;X;E9;X;R5",
            "M13\"Run the first 8 miles at a comfortable pace and then the last 5 miles at target pace\";X;T10\"Run the first 5 miles at a comfortable pace and then the last 5 miles at target pace\";X;E6;X;R6",
            "L18;X;E6;T8\"Run the first 3 miles at a comfortable pace and then the last 5 miles at target pace\";X;E8",
            "M16\"Run the first 8 miles at a comfortable pace and then the last 8 at target pace\";X;I5\"5 miles at target pace but take 60 second walking breaks in between each mile\";R5;X;E5;R4",
            "L18;X;E8;T8\"Run the first 3 miles at a comfortable pace and then the last 5 miles at target pace\";X;E10",
            // peak phase
            "L20;X;R7;T10\"Run the first 3 miles at a comfortable pace and then the last 7 miles at target pace\";X;E8;R5",
            "M18\"Run the first 8 miles at a comfortable pace and then the last 10 at target pace\";X;E7;I8\"8 miles at target pace but take 60 second walking breaks in between each mile\";X;L13;R4",
            "L16;X;E8;L11;X;T6;R4",
            "L17;X;R7;I7\"7 miles at target pace but take 60 second walking breaks in between each mile\";X;L11;R4",
            // taper phase
            "E10;X;E8;R6;X;R4;I5\"5 miles at target pace but take 60 second walking breaks in between each mile\"",
            "E5;X;R5;T5;X;R5;X",
            "T4;X;R4;M7\"Run the first 5 miles at a comfortable pace and then the last 2 miles at target pace\";X;X;R4;X",
        };

        public MarathonWorkoutPlanStrategy( DateTime raceDate, TimeSpan targetTime) : base(raceDate, targetTime) // Pass the dependencies to the base class constructor
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
                    // throw;
                }

                currentWeekStart = currentWeekStart.AddDays(7);
            }

            return workouts.ToArray();
        }

    }
}
