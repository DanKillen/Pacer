
using Pacer.Data.Entities;
using Pacer.Data.Services;
using Pacer.Data.Extensions;

namespace Pacer.Data.Strategies
{
    public class HalfMarathonWorkoutPlanStrategy : BaseWorkoutPlanStrategy
    {
        
        public HalfMarathonWorkoutPlanStrategy(RunningProfile runningProfile, DateTime startDate, DateTime endDate, TimeSpan targetTime)
        : base(runningProfile, startDate, endDate, targetTime, RaceType.HalfMarathon)
        {
        }

        public override Workout[] GenerateWorkouts()
        {
            var workouts = new List<Workout>();
            DateTime currentWeekStart = StartDate;

            for (int week = 0; week < 12; week++)
            {

                if (week < 3) // Base Building
                {
                    workouts.AddRange(GenerateBaseBuildingWorkouts(currentWeekStart, week));
                }
                else if (week < 6) // Support
                {
                    workouts.AddRange(GenerateSupportWorkouts(currentWeekStart, week));
                }
                else if (week < 10) // Race Specific
                {
                    workouts.AddRange(GenerateRaceSpecificWorkouts(currentWeekStart, week));
                }
                else // Taper
                {
                    workouts.AddRange(GenerateTaperWorkouts(currentWeekStart, week));
                }
                currentWeekStart = currentWeekStart.AddDays(7);
            }

            return workouts.ToArray();
        }

        private IEnumerable<Workout> GenerateBaseBuildingWorkouts(DateTime currentWeekStart, int week)
        {
            var workouts = new List<Workout>();

            workouts.Add(CreateWorkout(
                WorkoutType.EasyRun,
                currentWeekStart,
                10,
                EasyRunPace));

            workouts.Add(CreateWorkout(
                WorkoutType.LongRun,
                currentWeekStart.AddDays(2),
                20,
                LongRunPace));

            workouts.Add(CreateWorkout(
                WorkoutType.IntervalTraining,
                currentWeekStart.AddDays(4),
                5,
                IntervalTrainingPace));

            return workouts;
        }

        private IEnumerable<Workout> GenerateSupportWorkouts(DateTime currentWeekStart, int week)
        {
            var workouts = new List<Workout>();

            workouts.Add(CreateWorkout(
                WorkoutType.EasyRun,
                currentWeekStart,
                10,
                EasyRunPace));

            workouts.Add(CreateWorkout(
                WorkoutType.LongRun,
                currentWeekStart.AddDays(2),
                20,
                LongRunPace));

            workouts.Add(CreateWorkout(
                WorkoutType.IntervalTraining,
                currentWeekStart.AddDays(4),
                5,
                IntervalTrainingPace));

            return workouts;
        }

        private IEnumerable<Workout> GenerateRaceSpecificWorkouts(DateTime currentWeekStart, int week)
        {
            var workouts = new List<Workout>();

            workouts.Add(CreateWorkout(
                WorkoutType.EasyRun,
                currentWeekStart,
                10,
                EasyRunPace));

            workouts.Add(CreateWorkout(
                WorkoutType.LongRun,
                currentWeekStart.AddDays(2),
                20,
                LongRunPace));

            workouts.Add(CreateWorkout(
                WorkoutType.IntervalTraining,
                currentWeekStart.AddDays(4),
                5,
                IntervalTrainingPace));

            return workouts;
        }

        private IEnumerable<Workout> GenerateTaperWorkouts(DateTime currentWeekStart, int week)
        {
            var workouts = new List<Workout>();

            workouts.Add(CreateWorkout(
                WorkoutType.EasyRun,
                currentWeekStart,
                10,
                EasyRunPace));

            workouts.Add(CreateWorkout(
                WorkoutType.LongRun,
                currentWeekStart.AddDays(2),
                20,
                LongRunPace));

            workouts.Add(CreateWorkout(
                WorkoutType.IntervalTraining,
                currentWeekStart.AddDays(4),
                5,
                IntervalTrainingPace));

            return workouts;
        }
    }
}

