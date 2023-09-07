
using Pacer.Data.Entities;
using Pacer.Data.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Pacer.Data.Services
{
    public class TrainingPlanServiceDb : ITrainingPlanService
    {
        private readonly IDatabaseContext _ctx;
        private readonly ILogger<TrainingPlanServiceDb> _logger;
        private readonly IWorkoutFactory _workoutFactory;
        private readonly IWorkoutPaceCalculator _workoutPaceCalculator;
        private readonly IRunningProfileService _runningProfileService;

        public TrainingPlanServiceDb(IDatabaseContext ctx, ILogger<TrainingPlanServiceDb> logger, IWorkoutFactory workoutFactory, IRunningProfileService runningProfileService, IWorkoutPaceCalculator workoutPaceCalculator)
        {
            _ctx = ctx;
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
            if (runningProfile == null)
            {
                throw new ArgumentException($"No running profile found with id {runningProfileId}");
            }
            if (targetTime.TotalHours < 1 || targetTime.TotalHours > 5)
            {
                throw new ArgumentException("Target time must be between 1 and 5 hours.");
            }
            _ctx.SetEntityState(runningProfile, EntityState.Unchanged);
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

            _ctx.TrainingPlans.Add(newTrainingPlan);
            var age = DateTime.Now.Year - runningProfile.DateOfBirth.Year;
            var paces = _workoutPaceCalculator.CalculatePaces(targetTime, targetRace, age, runningProfile.Gender);
            foreach (var pace in paces)
            {
                newTrainingPlan.Paces.Add(pace);
            }

            _ctx.SaveChanges();
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

        public bool EditTargetTime(int trainingPlanId, TimeSpan targetTime)
        {
            var trainingPlan = GetOnlyPlanById(trainingPlanId);
            if (trainingPlan == null)
            {
                _logger.LogWarning($"No training plan found with id {trainingPlanId}");
                return false;
            }
            var runningProfile = _runningProfileService.GetProfileByProfileId(trainingPlan.RunningProfileId);
            if (runningProfile == null)
            {
                _logger.LogWarning($"No running profile found with id {trainingPlan.RunningProfileId}");
                return false;
            }
            using var transaction = _ctx.Database.BeginTransaction();
            try
            {
                var age = DateTime.Now.Year - runningProfile.DateOfBirth.Year;
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

                var paces = _workoutPaceCalculator.CalculatePaces(targetTime, trainingPlan.TargetRace, age, runningProfile.Gender);
                foreach (var pace in paces)
                {
                    trainingPlan.Paces.Add(pace);
                }

                _ctx.SaveChanges();

                transaction.Commit();  // Commit the transaction if all operations were successful
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();  // Rollback the transaction in case of an exception
                _logger.LogError($"Error editing target time: {ex.Message}");
                return false;
            }
        }

        public string[] GetRecommendation(TimeSpan estimatedMarathonTime, TimeSpan estimatedHalfMarathonTime, double weeklyMileage, DateTime dateOfBirth)
        {
            int age = DateTime.Now.Year - dateOfBirth.Year;
            string recommendation;
            bool slowMarathonTime = estimatedMarathonTime.TotalHours > 5;
            bool slowHalfMarathonTime = estimatedHalfMarathonTime.TotalHours > 2.5;

            if (age > 70)
            {
                recommendation = "Given your age, we would advise against the longer distances here. If you wish to continue, we suggest our Beginner Training Plans tailored for shorter distances.";
            }
            else if (estimatedMarathonTime.TotalHours < 3.5 || estimatedHalfMarathonTime.TotalHours < 1.5)
            {
                recommendation = weeklyMileage > 20 ?
                    "Your strong estimated times and weekly mileage make you a good candidate for our Standard Training Plans." :
                    "Your estimated times are promising, but we suggest increasing your weekly mileage for optimal results. Consider our Beginner Training Plans.";
            }
            else if (slowMarathonTime || slowHalfMarathonTime)
            {
                string raceTypeAdvice = slowMarathonTime ? "marathon" : "half-marathon";
                if (slowMarathonTime && slowHalfMarathonTime) raceTypeAdvice = "both marathon and half-marathon";
                recommendation = $"Your estimated times for the {raceTypeAdvice} suggest you could benefit from focusing on improving your fitness before attempting a longer race. If you wish to continue, we recommend our Beginner Training Plans.";
            }
            else
            {
                recommendation = "Your current performance indicates that our Beginner Training Plans would be a suitable starting point.";
            }
            return new string[]
            {
                $"Estimated Marathon Time: {estimatedMarathonTime.Hours}:{estimatedMarathonTime.Minutes:D2}.",
                $"Estimated Half Marathon Time: {estimatedHalfMarathonTime.Hours}:{estimatedHalfMarathonTime.Minutes:D2}.",
                recommendation,
                "Please note, these are estimations and actual race performance can vary."
            };
        }

        public TrainingPlan GetPlanById(int id)
        {
            return _ctx.TrainingPlans.AsSplitQuery()
                                    .Include(plan => plan.Workouts)
                                    .Include(plan => plan.Paces)  // Eager load the related TrainingPlanPaces
                                    .FirstOrDefault(plan => plan.Id == id);
        }
        private TrainingPlan GetOnlyPlanById(int id)
        {
            return _ctx.TrainingPlans.Find(id);
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
            return _ctx.TrainingPlans.AsSplitQuery()
                                    .Include(plan => plan.Workouts)
                                    .Include(plan => plan.Paces)  // Eager load the related TrainingPlanPaces
                                    .FirstOrDefault(plan => plan.RunningProfileId == profile.Id);
        }

        // Update a training plan
        public TrainingPlan UpdatePlan(TrainingPlan plan)
        {
            _ctx.TrainingPlans.Update(plan);
            _ctx.SaveChanges();
            return plan;
        }

        // Delete a training plan
        public bool DeletePlan(TrainingPlan plan)
        {
            _ctx.TrainingPlans.Remove(plan);
            _ctx.SaveChanges();
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
                    _ctx.SaveChanges();
                    return true;
                }
                else
                {
                    _logger.LogWarning($"No workout found with id {workoutId}");
                    return false;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error saving workout actuals: {e.Message}");
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
                    _logger.LogWarning($"No training plan found for user id: {userId}");
                    return false;
                }

                var workout = trainingPlan.Workouts.FirstOrDefault(w => w.Id == workoutId);

                if (workout == null)
                {
                    _logger.LogWarning($"No workout found with id {workoutId}");
                    return false;
                }

                workout.ActualDistance = 0;
                workout.ActualTime = new TimeSpan(0, 0, 0);

                _ctx.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Error clearing workout actuals: {e.Message}");
                return false;
            }
        }

    }

}