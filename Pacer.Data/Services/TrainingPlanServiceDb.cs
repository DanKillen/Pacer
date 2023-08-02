
using Pacer.Data.Entities;
using Pacer.Data.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Pacer.Data.Services
{
    public class TrainingPlanServiceDb : ITrainingPlanService
    {
        private readonly DatabaseContext ctx;
        private readonly ILogger<TrainingPlanServiceDb> _logger;
        private readonly IRunningProfileService _runningProfileService;

        public TrainingPlanServiceDb(DatabaseContext ctx, ILogger<TrainingPlanServiceDb> logger, IRunningProfileService runningProfileService)
        {
            this.ctx = ctx;
            _logger = logger;
            _runningProfileService = runningProfileService;
        }

        private static readonly Dictionary<RaceType, double> RaceDistancesInMiles = new Dictionary<RaceType, double>
        {
        { RaceType.FiveK, 3.1 },
        { RaceType.TenK, 6.2 },
        { RaceType.HalfMarathon, 13.1 },
        { RaceType.Marathon, 26.2 },
        };

        // ------------------ Training Plan Related Operations ------------------------

        // ---------------- Training Plan Management --------------

        public TrainingPlan CreatePlan(int runningProfileId, RaceType targetRace, DateTime raceDate, TimeSpan targetTime)
        {
            RunningProfile runningProfile = ctx.RunningProfiles.Find(runningProfileId);
            if (runningProfile == null)
            {
                throw new ArgumentException("No running profile found for the given id");
            }
            // Use WorkoutFactory to create workouts
            WorkoutFactory workoutFactory = new WorkoutFactory();
            Workout[] workouts = workoutFactory.AssignWorkouts(runningProfile, targetRace, raceDate, targetTime);

            string targetPace = CalculateTargetPace(targetRace, targetTime);


            TrainingPlan newTrainingPlan = new TrainingPlan
            {
                RunningProfile = runningProfile,
                TargetRace = targetRace,
                TargetTime = targetTime,
                TargetPace = targetPace,
                RaceDate = raceDate,
                Workouts = workouts
            };

            ctx.TrainingPlans.Add(newTrainingPlan);
            ctx.SaveChanges();

            return newTrainingPlan;
        }

        public string CalculateTargetPace(RaceType targetRace, TimeSpan targetTime)
        {
            if (!RaceDistancesInMiles.TryGetValue(targetRace, out double distanceInMiles))
            {
                throw new ArgumentException("Invalid race type", nameof(targetRace));
            }

            double timeInMinutes = targetTime.TotalMinutes;
            double pace = timeInMinutes / distanceInMiles;

            TimeSpan paceTimeSpan = TimeSpan.FromMinutes(pace);

            // Format the pace in mm:ss per mile
            return string.Format("{0:D2}:{1:D2} /mi", paceTimeSpan.Minutes, paceTimeSpan.Seconds);
        }


        public TrainingPlan GetPlanById(int id)
        {
            return ctx.TrainingPlans.Include(tp => tp.Workouts).SingleOrDefault(tp => tp.Id == id);
        }

        // Get a training plan by user
        public TrainingPlan GetPlanByUserId(int userId)
        {
            RunningProfile profile = _runningProfileService.GetProfileByUserId(userId);
            if (profile == null)
            {
                // Log the issue, the logger should be injected through the constructor
                _logger.LogWarning($"No running profile found for user id: {userId}");

                return null; // or throw a custom exception, or return a default/fallback plan.
            }
            return ctx.TrainingPlans.Include(plan => plan.Workouts).FirstOrDefault(plan => plan.RunningProfileId == profile.Id);
        }

        // Update a training plan
        public TrainingPlan UpdatePlan(TrainingPlan plan)
        {
            ctx.TrainingPlans.Update(plan);
            ctx.SaveChanges();
            return plan;
        }

        // Delete a training plan
        public bool DeletePlan(TrainingPlan plan)
        {
            ctx.TrainingPlans.Remove(plan);
            ctx.SaveChanges();
            return true;
        }

        public bool SaveWorkoutActuals(int workoutId, int userId, double actualDistance, TimeSpan actualTime)
        {
            try
            {
                var trainingPlan = GetPlanByUserId(userId);
                var workout = trainingPlan.Workouts.FirstOrDefault(w => w.Id == workoutId);
                if (workout != null)
                {
                    workout.ActualDistance = actualDistance;
                    workout.ActualTime = actualTime;
                    Console.WriteLine("Workout Actuals Saved: " + workoutId + " " + actualDistance + " " + workout.ActualTime);
                    ctx.SaveChanges();
                    return true;
                }
                else
                {
                    Console.WriteLine("Workout not found.");
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }



        public bool ClearWorkoutActuals(int workoutId, int userId)
        {
            try
            {
                var trainingPlan = GetPlanByUserId(userId);

                if (trainingPlan == null)
                {
                    Console.WriteLine($"No training plan found for user {userId}");
                    return false;
                }

                var workout = trainingPlan.Workouts.FirstOrDefault(w => w.Id == workoutId);

                if (workout == null)
                {
                    Console.WriteLine($"No workout found with id {workoutId}");
                    return false;
                }

                workout.ActualDistance = 0;
                workout.ActualTime = new TimeSpan(0, 0, 0);

                ctx.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

    }

}