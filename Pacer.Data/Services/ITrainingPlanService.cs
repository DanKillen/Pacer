using Pacer.Data.Entities;

namespace Pacer.Data.Services
{

    // This interface describes the operations that a RunningProfileService class implementation should provide
    public interface ITrainingPlanService
    {
           // ---------------- Training Plan Management --------------

        // Create a new training plan
        TrainingPlan CreatePlan(int RunningProfileId, RaceType targetRace, DateTime startDate, DateTime endDate, TimeSpan targetTime);

        // Get a training plan by user
        TrainingPlan GetPlanByUserId(int userId);

        // Update a training plan
        TrainingPlan UpdatePlan(TrainingPlan plan);

        // Delete a training plan
        void DeletePlan(TrainingPlan plan);

    }
}