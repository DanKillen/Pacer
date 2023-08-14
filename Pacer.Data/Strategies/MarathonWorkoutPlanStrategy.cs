using Pacer.Data.Entities;
using Pacer.Data.Services;
using Pacer.Data.Utilities;

namespace Pacer.Data.Strategies
{
    public class MarathonWorkoutPlanStrategy : BaseWorkoutPlanStrategy
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
        * L - Long Run
        * M - Race Pace
        *
        * Notes:
        * Some runs have additional instructions in quotes. For instance, 'M13"Run the first 8 miles...' 
        * means that it's a 13-mile medium run with specific guidance for pacing.
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

        public MarathonWorkoutPlanStrategy(DateTime raceDate, TimeSpan targetTime) : base(raceDate, targetTime) // Pass the dependencies to the base class constructor
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
