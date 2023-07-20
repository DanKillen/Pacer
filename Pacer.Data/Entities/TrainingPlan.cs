using System;
using System.ComponentModel.DataAnnotations;

namespace Pacer.Data.Entities
{

    public enum RaceType
    {
        FiveK,
        TenK,
        HalfMarathon,
        Marathon
    }

    public class TrainingPlan
    {
        public int Id { get; set; }

        // RunningProfile entity reference - to link a training plan to a running profile
        public RunningProfile RunningProfile { get; set; }
        [Required]
        public int RunningProfileId { get; set; }

        // Training plan specific properties
        public RaceType TargetRace { get; set; }

        public TimeSpan TargetTime { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Collection of workouts for this training plan
        public ICollection<Workout> Workouts { get; set; }
    }


}