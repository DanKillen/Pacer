
using Pacer.Data.Entities;
using Pacer.Data.Services;
using Pacer.Data.Extensions;

namespace Pacer.Data.Strategies
{
    public class HalfMarathonWorkoutPlanStrategy : BaseWorkoutPlanStrategy
    {

        private readonly string[] weekPlans = {
            // base phase
            "X,E8,R3,E5,X,R5,L8",
            "X,LT8,X,E9,X,R4,L12",
            "X,E8,X,E10,X,R5,M13\"Run the first 8 miles at a comfortable pace and then the last 5 at Marathon pace\"",
            // build phase
            "X,LT10\"Run the first 5 miles at a comfortable pace and then the last 5 at Lactate Threshold pace\",R4,E6,X,E7,L18",
            "X,E8,LT8\"Run the first 3 miles at a comfortable pace and then the last 5 at Lactate Threshold pace\",L12,X,E8,L14",
            "X,E6,L12,I5\"Take 60 second walking breaks in between each mile\",X,R5,L20",
            "X,R7,LT10\"Run the first 3 miles at a comfortable pace and then the last 7 at Lactate Threshold pace\",X,E8,R5,M18\"Run the first 8 miles at a comfortable pace and then the last 10 at Marathon pace\"",
            // peak phase
            "X,E8,I8\"Take 60 second walking breaks in between each mile\",X,L13,R5,L16",
            "X,E8,L11,X,R4,LT6,L17",
            "X,R7,I7\"Take 60 second walking breaks in between each mile\",X,L11,R4,L20",
            // taper phase
            "X,E8,R6,X,R4,I5\"Take 60 second walking breaks in between each mile\",L16",
            "X,E7,LT5,X,R5,X,L12",
            "X,R6,M7\"Run the first 5 miles at a comfortable pace and then the last 2 at Marathon pace\",X,R5,R4,X",
        };
        
        public HalfMarathonWorkoutPlanStrategy(RunningProfile runningProfile, DateTime raceDate, TimeSpan targetTime)
            : base(runningProfile, raceDate, targetTime, RaceType.HalfMarathon)
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
