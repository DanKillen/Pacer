
using Pacer.Data.Entities;
using Pacer.Data.Services;
using Pacer.Data.Extensions;

namespace Pacer.Data.Strategies
{
    public class FiveKWorkoutPlanStrategy : BaseWorkoutPlanStrategy
    {

        private readonly string[] weekPlans = {
            // base phase
            "X,E2,R1,X,E2,R1,L1",
            "X,E3,R1,X,E3,R1,L2",
            "X,E3,V1\"Aim for a fast, nearly all-out pace\",X,E3,R2,L2",
            "X,E4,R1,X,E4,V1\"Aim for a fast, nearly all-out pace\",L3",
            // Build Phase
            "X,E4,R1,X,E4,V2\"Aim for a fast, nearly all-out pace\",L3",
            "X,E4,R1,X,E4,V2\"Aim for a fast, nearly all-out pace\",L4",
            "X,E5,R2,X,E4,V2\"Aim for a fast, nearly all-out pace\",L4",
            // Peak Phase
            "X,E5,R2,X,E4,V2\"Aim for a fast, nearly all-out pace\",L4",
            // Taper Phase
            "X,E3,R1,X,E2,R1,L3",
            "X,E3,R1,X,E2,R1,X"
        };
        public FiveKWorkoutPlanStrategy(RunningProfile runningProfile, DateTime raceDate, TimeSpan targetTime)
            : base(runningProfile, raceDate, targetTime, RaceType.FiveK)
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
