
using Pacer.Data.Entities;
using Pacer.Data.Repositories;
using Microsoft.Extensions.Logging;

namespace Pacer.Data.Services
{
    public class TrainingPlanServiceDb : ITrainingPlanService
    {
        private readonly DatabaseContext ctx;

        public TrainingPlanServiceDb(DatabaseContext ctx)
        {
            this.ctx = ctx;
        }

        // ------------------ Training Plan Related Operations ------------------------

        // ---------------- Training Plan Management --------------

        public TrainingPlan CreatePlan(int runningProfileId, RaceType targetRace, DateTime startDate, DateTime endDate, TimeSpan targetTime)
        {
            RunningProfile runningProfile = ctx.RunningProfiles.Find(runningProfileId);
            if (runningProfile == null)
            {
                throw new ArgumentException("No running profile found for the given id");
            }
            // Use WorkoutFactory to create workouts
            WorkoutFactory workoutFactory = new WorkoutFactory();
            Workout[] workouts = workoutFactory.AssignWorkouts(runningProfile, targetRace, startDate, endDate, targetTime);


            TrainingPlan newTrainingPlan = new TrainingPlan
            {
                RunningProfile = runningProfile,
                TargetRace = targetRace,
                TargetTime = targetTime,
                StartDate = startDate,
                EndDate = endDate,
                Workouts = workouts
            };

            ctx.TrainingPlans.Add(newTrainingPlan);
            ctx.SaveChanges();

            return newTrainingPlan;
        }

        // Get a training plan by user
        public TrainingPlan GetPlanByUserId(int userId)
        {
            RunningProfile runningProfile = ctx.RunningProfiles.Find(userId);
            if (runningProfile == null)
            {
                throw new ArgumentException("No running profile found for the given id");
            }
            return ctx.TrainingPlans.FirstOrDefault(plan => plan.RunningProfileId == runningProfile.Id);
        }

        // Update a training plan
        public TrainingPlan UpdatePlan(TrainingPlan plan)
        {
            ctx.TrainingPlans.Update(plan);
            ctx.SaveChanges();
            return plan;
        }

        // Delete a training plan
        public void DeletePlan(TrainingPlan plan)
        {
            ctx.TrainingPlans.Remove(plan);
            ctx.SaveChanges();
        }

    }

}