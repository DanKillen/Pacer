using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pacer.Data.Extensions;

namespace Pacer.Data.Entities
{

    public enum RaceType
    {
        [Display(Name = "Beginner Half Marathon")]
        BHalfMarathon,
        [Display(Name = "Half Marathon")]
        HalfMarathon,
        [Display(Name = "Beginner Marathon")]
        BMarathon,
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
        // Navigation collection for associated Pace entries
        // Navigation property for the paces
        public ICollection<TrainingPlanPace> Paces { get; set; }
    }

    public class TrainingPlanPace
    {
        public int Id { get; set; }
        public int TrainingPlanId { get; set; }
        public WorkoutType WorkoutType { get; set; }
        public PaceType PaceType { get; set; }  // Either Min or Max

        private PaceTime _pace;
        [NotMapped]
        public PaceTime Pace
        {
            get => _pace;
            set
            {
                _pace = value;
                PaceString = value.ToString();
            }
        }
        public string PaceString { get; set; }
        public PaceTime GetPaceTime()
        {
            var parts = PaceString.Split(':');
            return new PaceTime(new TimeSpan(0, int.Parse(parts[0]), int.Parse(parts[1])));
        }
        public TrainingPlan TrainingPlan { get; set; }
    }
    public enum PaceType
    {
        Min,
        Max
    }
}
