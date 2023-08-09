
using Pacer.Data.Entities;
using Pacer.Data.Services;
using Pacer.Data.Security;
using Pacer.Data.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Pacer.Data.Services
{
    public class RunningProfileServiceDb : IRunningProfileService
    {
        private readonly DatabaseContext _ctx;
        private readonly IRaceTimePredictor _raceTimePredictor;

        public RunningProfileServiceDb(DatabaseContext ctx, IRaceTimePredictor raceTimePredictor)
        {
            _ctx = ctx;
            _raceTimePredictor = raceTimePredictor;
        }

        public void Initialise()
        {
            _ctx.Initialise();
        }

        // ---------------- Running Profile Management --------------

        // Create a new running profile
        public RunningProfile CreateProfile(int userId, DateTime dateOfBirth, string gender, int weeklyMileage, TimeSpan fiveKTime)
        {
            // First, confirm that a User with the given userId exists
            User user = _ctx.Users.Find(userId) ?? throw new ArgumentException("No user found with the given userId");

            TimeSpan estimatedMarathonTime = _raceTimePredictor.CalculateEstimatedMarathonTime(fiveKTime);
            TimeSpan estimatedHalfMarathonTime = _raceTimePredictor.CalculateEstimatedHalfMarathonTime(fiveKTime);

            RunningProfile newProfile = new()
            {
                UserId = userId,
                DateOfBirth = dateOfBirth,
                Gender = gender,
                WeeklyMileage = weeklyMileage,
                FiveKTime = fiveKTime,
                EstimatedMarathonTime = estimatedMarathonTime,
                EstimatedHalfMarathonTime = estimatedHalfMarathonTime
            };

            _ctx.RunningProfiles.Add(newProfile);
            user.RunningProfile = newProfile;
            _ctx.SaveChanges();
            return newProfile;
        }

        // Get a running profile by id
        public RunningProfile GetProfileByUserId(int userId)
        {
            return _ctx.RunningProfiles.Include(rp => rp.User).FirstOrDefault(rp => rp.User.Id == userId);
        }
        // Update a running profile
        public RunningProfile UpdateProfile(int userId, DateTime dateOfBirth, string gender, int weeklyMileage, TimeSpan fiveKTime)
        {
            // Check if the profile exists in the context
            RunningProfile existingProfile = _ctx.RunningProfiles.FirstOrDefault(rp => rp.UserId == userId) ?? throw new ArgumentException("No running profile found with the given profile Id");
            TimeSpan estimatedMarathonTime = _raceTimePredictor.CalculateEstimatedMarathonTime(fiveKTime);
            TimeSpan estimatedHalfMarathonTime = _raceTimePredictor.CalculateEstimatedHalfMarathonTime(fiveKTime);

            // Update the profile properties
            existingProfile.DateOfBirth = dateOfBirth;
            existingProfile.Gender = gender;
            existingProfile.WeeklyMileage = weeklyMileage;
            existingProfile.FiveKTime = fiveKTime;
            existingProfile.EstimatedMarathonTime = estimatedMarathonTime;
            existingProfile.EstimatedHalfMarathonTime = estimatedHalfMarathonTime;

            _ctx.SaveChanges();

            return existingProfile;
        }
        // Delete a running profile
        public void DeleteProfile(RunningProfile profile)
        {
            // Check if the profile exists in the context
            RunningProfile existingProfile = _ctx.RunningProfiles.Find(profile.Id) ?? throw new ArgumentException("No running profile found with the given profile Id");
            _ctx.RunningProfiles.Remove(existingProfile);
            _ctx.SaveChanges();
        }

    }

}