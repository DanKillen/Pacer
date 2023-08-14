
using Pacer.Data.Entities;
using Pacer.Data.Services;
using Pacer.Data.Utilities;

namespace Pacer.Data.Strategies
{
    public class BeginnerHalfMarathonWorkoutPlanStrategy : BaseWorkoutPlanStrategy
    {

        private readonly string[] WeekPlans = {
            // base phase
            "X;X;E2;X;E3;X;R2",
            "E2;X;R2;X;E3;X;R2",
            "E3;X;T2;X;E3;X;R2",
            "E3;X;V2;X;E4;X;R2",
            "E3;X;T3;X;E4;X;R3",

            // build phase
            "E3;X;V2;X;E4;X;R4",
            "E5;X;T3;X;E4;X;R4",

            // peak phase
            "L7;X;T4;X;R4;X;R3",
            "L9;X;R4;X;I3\"3 miles at target pace but take 60 second walking breaks in between each mile\";X;R2",

            // taper phase
            "E4;X;R2;X;T2;X;R2",
        };
        public BeginnerHalfMarathonWorkoutPlanStrategy(DateTime raceDate, TimeSpan targetTime)
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
                }
                currentWeekStart = currentWeekStart.AddDays(7);
            }

            return workouts.ToArray();
        }

    }
}
