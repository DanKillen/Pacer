
using Pacer.Data.Entities;
using Pacer.Data.Services;
using Pacer.Data.Extensions;

namespace Pacer.Data.Strategies
{
    public class FiveKWorkoutPlanStrategy : BaseWorkoutPlanStrategy
    {

        private readonly string[] weekPlans = {
            // base phase
            "X;E2;R1;X;E2;R1;L1",
            "X;E3;R1;X;E3;R1;L2",
            "X;E3;V1\"Aim for a fast, nearly all-out pace. $pace or faster\";X;E3;R2;E2",
            "X;E4;R1;X;E4;V1\"Aim for a fast, nearly all-out pace. $pace or faster\";E3",
            // Build Phase
            "X;E4;R1;X;E4;V2\"Aim for a fast, nearly all-out pace. $pace or faster\";E3",
            "X;E4;R1;X;E4;V2\"Aim for a fast, nearly all-out pace. $pace or faster\";E4",
            "X;E5;R2;X;E4;V2\"Aim for a fast, nearly all-out pace. $pace or faster\";E4",
            // Peak Phase
            "X;E5;R2;X;E4;V2\"Aim for a fast, nearly all-out pace. $pace or faster\";E4",
            // Taper Phase
            "X;E3;R1;X;E2;R1;E3",
            "X;E3;R1;X;E2;R1;X"
        };
        public FiveKWorkoutPlanStrategy(RunningProfile runningProfile, DateTime raceDate, TimeSpan targetTime, IWorkoutPaceCalculator workoutPaceCalculator)
            : base(runningProfile, raceDate, targetTime, workoutPaceCalculator)
        {
            InitializePaces(RaceType.FiveK);
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
                }
                currentWeekStart = currentWeekStart.AddDays(7);
            }

            return workouts.ToArray();
        }

    }
}
