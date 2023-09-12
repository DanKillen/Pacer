using Pacer.Data.Entities;

namespace Pacer.Data.Services;


// This interface describes the operations that a RunningProfileService class implementation should provide
public interface ITrainingPlanService
{
    // ---------------- Training Plan Management --------------

    // Create a new training plan
    TrainingPlan CreatePlan(int RunningProfileId, RaceType targetRace, DateTime raceDate, TimeSpan targetTime);
    bool EditTargetTime(int trainingPlanId, TimeSpan targetTime);
    // Get training plan recommendations based on running profile
    string[] GetRecommendation(TimeSpan estimatedMarathonTime, TimeSpan estimatedHalfMarathonTime, double weeklyMileage, DateTime dateOfBirth, TimeSpan fiveKTime);
    // Get a training plan by id
    TrainingPlan GetPlanById(int Id);
    // Get a training plan by user
    TrainingPlan GetPlanByUserId(int userId);

    // Delete a training plan
    bool DeletePlan(TrainingPlan plan);

    // ---------------- Workout Management --------------
    bool SaveWorkoutActuals(int workoutId, int userId, double actualDistance, TimeSpan actualTime);

    bool ClearWorkoutActuals(int workoutId, int userId);
    // Methods used to change the date of a workout
    List<DateTime> GetAvailableDates(int workoutId, int userId);
    bool UpdateWorkoutDate(int workoutId, int userId, DateTime newDate);


}

