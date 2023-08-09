using System;
namespace Pacer.Data.Entities
{
    public class RunningProfile
    {
        public int Id { get; set; }

        // User entity reference - to link a user to a running profile
        public User User { get; set; }
        public int UserId { get; set; }

        // Running specific properties
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public int WeeklyMileage { get; set; }
        public TimeSpan FiveKTime { get; set; }
        public TimeSpan EstimatedMarathonTime { get; set; }
        public TimeSpan EstimatedHalfMarathonTime { get; set; }
    }
}

