
using Pacer.Data.Entities;
using Pacer.Data.Services;
using Pacer.Data.Utilities;

namespace Pacer.Data.Strategies
{
    public class HalfMarathonWorkoutPlanStrategy : BaseWorkoutPlanStrategy
    {

        private readonly string[] WeekPlans = {
            // base phase
            "X;X;E3;X;E3;R4;X",
            "E3;X;E2;X;E3;X;R2",
            "E3;X;E3;X;E4;X;R4",
            "E4;X;E3;X;E4;X;R5",

            // build phase
            "E4;X;T4;R3;E4;X;R3",
            "L6;X;E4;X;V2;X;E4;R4",
            "L8;X;E4;X;I4\"4 miles at target pace but take 60 second walking breaks in between each mile\";X;X;R4",

            // peak phase
            "L9;X;X;V3;X;E4;R5",
            "L10;X;R4;X;I5\"5 miles at target pace but take 60 second walking breaks in between each mile\";X;E3;R3",

            // taper phase
            "L6;X;T2;E2;X;R2;X;X",
        };

        public HalfMarathonWorkoutPlanStrategy(DateTime raceDate, TimeSpan targetTime)
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
