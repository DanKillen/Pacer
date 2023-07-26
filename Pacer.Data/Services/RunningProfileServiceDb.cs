
using Pacer.Data.Entities;
using Pacer.Data.Services;
using Pacer.Data.Security;
using Pacer.Data.Repositories;
using Microsoft.Extensions.Logging;

namespace Pacer.Data.Services
{
    public class RunningProfileServiceDb : IRunningProfileService
    {
        private readonly DatabaseContext _ctx;
        private readonly IScoreCalculator _scoreCalculator;

        public RunningProfileServiceDb(DatabaseContext ctx, IScoreCalculator scoreCalculator)
        {
            _ctx = ctx;
            _scoreCalculator = scoreCalculator;
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
            User user = _ctx.Users.Find(userId);
            if (user == null)
            {
                throw new ArgumentException("No user found with the given userId");
            }

            double anaerobicScore = _scoreCalculator.CalculateInitialAnaerobicScore(fiveKTime);
            double aerobicScore = _scoreCalculator.CalculateInitialAerobicScore(weeklyMileage, fiveKTime);

            RunningProfile newProfile = new RunningProfile
            {
                UserId = userId,
                DateOfBirth = dateOfBirth,
                Gender = gender,
                WeeklyMileage = weeklyMileage,
                FiveKTime = fiveKTime,
                AnaerobicScore = anaerobicScore,
                AerobicScore = aerobicScore
            };

            _ctx.RunningProfiles.Add(newProfile);
            _ctx.SaveChanges();
            return newProfile;
        }

        // Get a running profile by id
        public RunningProfile GetProfileByUserId(int userId)
        {
            return _ctx.RunningProfiles.FirstOrDefault(rp => rp.User.Id == userId);
        }
        // Update a running profile
        public RunningProfile UpdateProfile(int userId, DateTime dateOfBirth, string gender, int weeklyMileage, TimeSpan fiveKTime)
        {
            // Check if the profile exists in the context
            RunningProfile existingProfile = _ctx.RunningProfiles.Find(userId);
            if (existingProfile == null)
            {
                throw new ArgumentException("No running profile found with the given profile Id");
            }

            double anaerobicScore = _scoreCalculator.CalculateInitialAnaerobicScore(fiveKTime);
            double aerobicScore = _scoreCalculator.CalculateInitialAerobicScore(weeklyMileage, fiveKTime);

            // Update the profile properties
            existingProfile.DateOfBirth = dateOfBirth;
            existingProfile.Gender = gender;
            existingProfile.WeeklyMileage = weeklyMileage;
            existingProfile.FiveKTime = fiveKTime;
            existingProfile.AnaerobicScore = anaerobicScore;
            existingProfile.AerobicScore = aerobicScore;

            _ctx.SaveChanges();

            return existingProfile;
        }
        // Delete a running profile
        public void DeleteProfile(RunningProfile profile)
        {
            // Check if the profile exists in the context
            RunningProfile existingProfile = _ctx.RunningProfiles.Find(profile.Id);
            if (existingProfile == null)
            {
                throw new ArgumentException("No running profile found with the given profile Id");
            }

            _ctx.RunningProfiles.Remove(existingProfile);
            _ctx.SaveChanges();
        }
        
    }

}