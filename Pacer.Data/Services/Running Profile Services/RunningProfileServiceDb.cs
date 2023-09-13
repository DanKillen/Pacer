
using Pacer.Data.Entities;
using Pacer.Data.Services;
using Pacer.Data.Security;
using Pacer.Data.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Pacer.Data.Services;

    public class RunningProfileServiceDb : IRunningProfileService
    {
        private readonly IDatabaseContext _ctx;
        private readonly IRaceTimePredictor _raceTimePredictor;

        public RunningProfileServiceDb(IDatabaseContext ctx, IRaceTimePredictor raceTimePredictor)
        {
            _ctx = ctx;
            _raceTimePredictor = raceTimePredictor;
        }
        // ---------------- Running Profile Management --------------

        // Create a new running profile
        public RunningProfile CreateProfile(int userId, DateTime dateOfBirth, string gender, int weeklyMileage, TimeSpan fiveKTime)
        {
            // Confirm that a User with the given userId exists
            User user = _ctx.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                
                return null;
            }
            // 
            var age = DateTime.Now.Year - dateOfBirth.Year;
            if (age < 18 || age > 85)
            {
                throw new ArgumentException("DateOfBirth must correspond to an age between 18 and 85.");
            }
            if (fiveKTime.TotalMinutes < 12.5 || fiveKTime.TotalMinutes > 30)
            {
                throw new ArgumentException("FiveKTime must be between 12.5 and 30 minutes.");
            }
            if (weeklyMileage < 0 || weeklyMileage > 200)
            {
                throw new ArgumentException("WeeklyMileage must be between 0 and 200.");
            }
            TimeSpan estimatedMarathonTime = _raceTimePredictor.CalculateEstimatedMarathonTime(age, fiveKTime, gender);
            TimeSpan estimatedHalfMarathonTime = _raceTimePredictor.CalculateEstimatedHalfMarathonTime(age, fiveKTime, gender);

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

        // Get a running profile by User id
        public RunningProfile GetProfileByUserId(int userId)
        {
            return _ctx.RunningProfiles.Include(rp => rp.User).FirstOrDefault(rp => rp.User.Id == userId);
        }

        // Get a running profile by profile id
        public RunningProfile GetProfileByProfileId(int profileId)
        {
            return _ctx.RunningProfiles.Include(rp => rp.User).FirstOrDefault(rp => rp.Id == profileId);
        }

        // Update a running profile
        public RunningProfile UpdateProfile(int userId, DateTime dateOfBirth, string gender, int weeklyMileage, TimeSpan fiveKTime)
        {
            // Check if the profile exists in the context
            RunningProfile existingProfile = _ctx.RunningProfiles.FirstOrDefault(rp => rp.UserId == userId) ?? throw new ArgumentException("No running profile found with the given profile Id");
            var age = DateTime.Now.Year - dateOfBirth.Year;
            if (age < 18 || age > 85)
            {
                throw new ArgumentException("DateOfBirth must correspond to an age between 18 and 85.");
            }
            if (fiveKTime.TotalMinutes < 12.5 || fiveKTime.TotalMinutes > 30)
            {
                throw new ArgumentException("FiveKTime must be between 12.5 and 30 minutes.");
            }
            if (weeklyMileage < 0 || weeklyMileage > 200)
            {
                throw new ArgumentException("WeeklyMileage must be between 0 and 200.");
            }
            // Calculate the estimated marathon and half marathon times
            TimeSpan estimatedMarathonTime = _raceTimePredictor.CalculateEstimatedMarathonTime(age, fiveKTime, gender);
            TimeSpan estimatedHalfMarathonTime = _raceTimePredictor.CalculateEstimatedHalfMarathonTime(age, fiveKTime, gender);

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

    }

