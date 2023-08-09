
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
        private readonly IWorkoutFactory _workoutFactory;
        private readonly IWorkoutPaceCalculator _workoutPaceCalculator;
        private readonly IRunningProfileService _runningProfileService;

        public TrainingPlanServiceDb(DatabaseContext ctx, ILogger<TrainingPlanServiceDb> logger, IWorkoutFactory workoutFactory, IRunningProfileService runningProfileService, IWorkoutPaceCalculator workoutPaceCalculator)
        {
            this.ctx = ctx;
            _logger = logger;
            _workoutFactory = workoutFactory;
            _runningProfileService = runningProfileService;
            _workoutPaceCalculator = workoutPaceCalculator;
        }

        private static readonly Dictionary<RaceType, double> RaceDistancesInMiles = new()
        {
        { RaceType.BHalfMarathon, 13.1 },
        { RaceType.HalfMarathon, 13.1 },
        { RaceType.BMarathon, 26.2 },
        { RaceType.Marathon, 26.2 },
        };

        // ------------------ Training Plan Related Operations ------------------------

        // ---------------- Training Plan Management --------------

        public TrainingPlan CreatePlan(int runningProfileId, RaceType targetRace, DateTime raceDate, TimeSpan targetTime)
        {

            var runningProfile = _runningProfileService.GetProfileByProfileId(runningProfileId);
            var workouts = _workoutFactory.AssignWorkouts(targetRace, raceDate, targetTime);
            var targetPace = CalculateTargetPace(targetRace, targetTime);

            TrainingPlan newTrainingPlan = new()
            {
                RunningProfile = runningProfile,
                TargetRace = targetRace,
                TargetTime = targetTime,
                TargetPace = targetPace,
                RaceDate = raceDate,
                Workouts = workouts,
                Paces = new List<TrainingPlanPace>()
            };


            ctx.TrainingPlans.Add(newTrainingPlan);

            var paces = _workoutPaceCalculator.CalculatePaces(targetTime, targetRace);
            foreach (var pace in paces)
            {
                newTrainingPlan.Paces.Add(pace);
            }

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
            return string.Format("{0}:{1:D2}", paceTimeSpan.Minutes, paceTimeSpan.Seconds);
        }

        public bool EditTargetTime(int trainingPlanId, RaceType targetRace, TimeSpan targetTime)
        {
            using var transaction = ctx.Database.BeginTransaction();
            try
            {
                var trainingPlan = GetPlanById(trainingPlanId);
                if (trainingPlan == null)
                {
                    Console.WriteLine($"No training plan found with id {trainingPlanId}");
                    return false;
                }

                trainingPlan.TargetTime = targetTime;
                trainingPlan.TargetPace = CalculateTargetPace(trainingPlan.TargetRace, targetTime);

                if (trainingPlan.Paces != null)
                {
                    trainingPlan.Paces.Clear();
                }
                else
                {
                    trainingPlan.Paces = new List<TrainingPlanPace>();
                }

                var paces = _workoutPaceCalculator.CalculatePaces(targetTime, trainingPlan.TargetRace);
                foreach (var pace in paces)
                {
                    trainingPlan.Paces.Add(pace);
                }

                ctx.SaveChanges();

                transaction.Commit();  // Commit the transaction if all operations were successful
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();  // Rollback the transaction in case of an exception
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

        public string[] GetRecommendation(TimeSpan estimatedMarathonTime, TimeSpan estimatedHalfMarathonTime, double weeklyMileage, DateTime dateOfBirth)
        {
            int age = DateTime.Now.Year - dateOfBirth.Year;
            string marathonRecommendation;

            // Marathon Recommendations
            if (age > 70)
            {
                marathonRecommendation = "Given your age, we would recommend our Beginner Training Plans and would discourage you from a whole Marathon.";
            }
            else if (estimatedMarathonTime.TotalHours < 3.5)
            {
                marathonRecommendation = weeklyMileage > 20 ?
                    "Given your weekly mileage and estimated pace, we recommend the Standard Training Plans for you." :
                    "While you have promising estimated times, we would be concerned about the lack of weekly mileage. At this stage we would recommend our Beginner Training Plans.";
            }
            else if (estimatedMarathonTime.TotalHours < 4)
            {
                marathonRecommendation = weeklyMileage >= 20 ?
                    (age < 45 ?
                        "Considering your age and performance, our Standard Training Plans seem suitable." :
                        "Given your age and performance, the Beginner Plans might be more appropriate. However, if you have past marathon experience, the Standard Plan remains an option.") :
                        "Your estimated times suggest potential, but building up your mileage will benefit your training. We advise starting with our Beginner Training Plans.";
            }
            else
            {
                marathonRecommendation = "Given your current estimated times, we suggest starting with one of our Beginner Training Plans.";
            }

            return new string[]
            {
        $"Estimated Marathon Time: {estimatedMarathonTime.Hours}:{estimatedMarathonTime.Minutes:D2}.",
        $"Estimated Half Marathon Time: {estimatedHalfMarathonTime.Hours}:{estimatedHalfMarathonTime.Minutes:D2}.",
        marathonRecommendation,
        "These are just estimations. Actual race performance can vary based on a myriad of factors."
            };
        }

        public TrainingPlan GetPlanById(int id)
        {
            return ctx.TrainingPlans.AsSplitQuery()
                                    .Include(plan => plan.Workouts)
                                    .Include(plan => plan.Paces)  // Eager load the related TrainingPlanPaces
                                    .FirstOrDefault(plan => plan.Id == id);
        }

        // Get a training plan by user
        public TrainingPlan GetPlanByUserId(int userId)
        {
            RunningProfile profile = _runningProfileService.GetProfileByUserId(userId);
            if (profile == null)
            {
                _logger.LogWarning($"No running profile found for user id: {userId}");

                return null;
            }
            return ctx.TrainingPlans.AsSplitQuery()
                                    .Include(plan => plan.Workouts)
                                    .Include(plan => plan.Paces)  // Eager load the related TrainingPlanPaces
                                    .FirstOrDefault(plan => plan.RunningProfileId == profile.Id);
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