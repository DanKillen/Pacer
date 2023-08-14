
using Pacer.Data.Entities;
using Pacer.Data.Services;
using Pacer.Data.Utilities;

namespace Pacer.Data.Strategies
{
    public class HalfMarathonWorkoutPlanStrategy : BaseWorkoutPlanStrategy
    {
        /*
        * WeekPlans Key:
        * Each string in the WeekPlans array represents a week's workout plan. Every day's workout is separated by a semicolon.
        * Each workout has the symbol for the type of run followed by the distance in miles.
        * The phases of training (base, build, peak, taper) are grouped together, and each phase has a specific focus for the athlete.
        *
        * Symbols and Abbreviations:
        * E - Easy Run
        * R - Recovery Run
        * X - Rest
        * I - Interval Training
        * T - Tempo Run
        * V - VO2 Max
        * L - Long Run
        *
        * Notes:
        * Some runs have additional instructions in quotes. For instance, 'I4"4 miles at target pace but take 60...' 
        * means that it's a 4-mile Interval Training run with additional guidance in the description.
        * 
        * Phases:
        * Base Phase: This is the foundation phase where the athlete builds up mileage and gets ready for more specific workouts.
        * Build Phase: This phase introduces more specialized workouts, and the mileage increases.
        * Peak Phase: The most intensive phase where the athlete reaches their highest mileage and does specific workouts to 
        * prepare for the race.
        * Taper Phase: Mileage and intensity are reduced to allow the athlete to recover and be fresh for the race.
        */

        private readonly string[] WeekPlans = {
            // base phase
            "X;X;E3;X;E3;R4;X",
            "E3;X;E2;X;E3;X;R2",
            "E3;X;E3;X;E4;X;R4",
            "E4;X;E3;X;E4;X;R5",

            // build phase
            "E4;X;T4;R3;E4;X;R3",
            "L6;X;E4;X;V3;X;E4;R4",
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
