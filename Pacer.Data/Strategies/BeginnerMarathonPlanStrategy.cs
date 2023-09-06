
using Pacer.Data.Entities;
using Pacer.Data.Services;
using Pacer.Data.Utilities;
using Microsoft.Extensions.Logging;

namespace Pacer.Data.Strategies
{
   
    public class BeginnerMarathonPlanStrategy : BasePlanStrategy
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
        * Some runs have additional instructions in quotes. For instance, 'I4\"4 miles at target pace but take 60...' 
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
            "E4;R2;X;E4;X;L5;X",
            "E5;R3;X;E4;X;L5;X",
            "E4;R3;X;E5;X;L7;X",
            "E4;X;E4;R2;E5;X;R4",
            "E5;X;E4;R2;E5;X;R5",
            "L8;X;E4;R3;E5;X;R5",

            // build phase
            "L10;X;E5;X;E6;X;R5",
            "L12;X;E5;X;T5\"Maintain a consistent, slightly challenging pace\";X;E5",
            "L15;X;I4\"Run fast for 1 minute, then walk or jog for 2 minutes. Repeat.\";X;E5;X;R5",
            "L16;X;E6;X;T7\"Maintain a consistent, slightly challenging pace\";X;E6",

            // peak phase
            "L20;X;I5\"Run target pace for 1 minute, then walk or jog for 2 minutes. Repeat.\";X;E6;X;R5",
            "M14\"Run 4 miles at a comfortable pace, and then 10 miles at target pace\";X;E6;X;T8\"Maintain a consistent, slightly challenging pace\";X;E7",
            "L20;X;E5;X;I5\"Run target pace for 1 minute, then walk or jog for 2 minutes. Repeat.\";X;R5",

            // taper phase
            "L14;X;E6;X;T6\"Maintain a consistent, slightly challenging pace\";X;R5",
            "L8;X;T5\"Maintain a consistent, slightly challenging pace\";X;E5;X;R4",
            "L6;X;R4;M4\"Run 2 miles at a comfortable pace, and then 2 miles at target pace\";X;R3;R2",

        };

        public BeginnerMarathonPlanStrategy(DateTime raceDate, TimeSpan targetTime)
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
