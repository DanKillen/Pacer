
using Pacer.Data.Entities;
using Pacer.Data.Services;
using Pacer.Data.Utilities;
using Microsoft.Extensions.Logging;

namespace Pacer.Data.Strategies
{
    public class BeginnerMarathonWorkoutPlanStrategy : BaseWorkoutPlanStrategy
    {
        private readonly string[] WeekPlans = {
            // base phase
            "E4;R2;X;E4;X;L5;X",
            "E5;R3;X;E4;X;L5;X",
            "E4;R3;X;E5;X;L7;X",
            "E4;X;E4;R2;E5;X;R4",
            "E5;X;E4;R2;E5;X;R5",
            "L7;X;E4;R3;E5;X;R5",

            // build phase
            "L8;X;E5;R3;E6;X;R5",
            "L9;X;E5;T6\"Maintain a consistent pace\";X;E6;X;R5",
            "L10;X;I4\"Run fast for 1 minute, then walk or jog for 2 minutes. Repeat.\";R4;E5;X;R5",
            "L12;X;E6;R4;T7\"Maintain a consistent, slightly challenging pace\";X;E6;R5",

            // peak phase
            "L14;X;E6;I5\"Run fast for 1 minute, then walk or jog for 2 minutes. Repeat.\";R4;E7;X;R5",
            "L15;X;E6;R5;T8\"Maintain a consistent, slightly challenging pace\";X;E7;X",
            "L16;X;E6;I5\"Run fast for 1 minute, then walk or jog for 2 minutes. Repeat.\";R5;E7;X;R5",

            // taper phase
            "L14;X;E6;R4;T6\"Maintain a consistent, slightly challenging pace\";X;E5;X",
            "L12;X;E5;R4;T5\"Maintain a consistent, slightly challenging pace\";X;E4;X",
            "L8;X;E4;M4\"Run at a comfortable pace to shake out the legs\";X;R3;R3;X",

        };

        public BeginnerMarathonWorkoutPlanStrategy(DateTime raceDate, TimeSpan targetTime)
        : base(raceDate, targetTime) // Pass the dependencies to the base class constructor
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
