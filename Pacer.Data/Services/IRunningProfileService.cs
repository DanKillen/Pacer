
using Pacer.Data.Entities;

namespace Pacer.Data.Services
{

    // This interface describes the operations that a RunningProfileService class implementation should provide
    public interface IRunningProfileService
    {
        // ---------------- Running Profile Management --------------

        // Create a new running profile
        RunningProfile CreateProfile(int userId, DateTime dateOfBirth, string gender, int weeklyMileage, TimeSpan fiveKTime);

        // Get a running profile by user
        RunningProfile GetProfileByUserId(int userId);

        // Update a running profile
        RunningProfile UpdateProfile(RunningProfile profile);

        // Delete a running profile
        void DeleteProfile(RunningProfile profile);    
        
    }
}