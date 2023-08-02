using System;
using System.ComponentModel.DataAnnotations;

namespace Pacer.Data.Entities
{

    public enum RaceType
    {
        [Display(Name = "5K")]
        FiveK,
        [Display(Name = "10K")]
        TenK,
        [Display(Name = "Half Marathon")]
        HalfMarathon,
        [Display(Name = "Marathon")]
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
        public string TargetPace { get; set; }

        public DateTime RaceDate { get; set; }

        // Collection of workouts for this training plan
        public ICollection<Workout> Workouts { get; set; }
    }


}