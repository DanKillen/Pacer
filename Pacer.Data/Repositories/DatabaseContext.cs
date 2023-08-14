using Microsoft.EntityFrameworkCore;

// import the Entities (database models representing structure of tables in database)
using Pacer.Data.Entities;

namespace Pacer.Data.Repositories
{
    // The Context is How EntityFramework communicates with the database
    // We define DbSet properties for each table in the database
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<ForgotPassword> ForgotPasswords { get; set; }
        public DbSet<RunningProfile> RunningProfiles { get; set; }
        public DbSet<TrainingPlan> TrainingPlans { get; set; }
        public DbSet<Workout> Workouts { get; set; }
        public DbSet<TrainingPlanPace> Paces { get; set; }
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // TrainingPlan entity configuration
            modelBuilder.Entity<TrainingPlan>()
                        .HasKey(e => e.Id);  // Setting the primary key for TrainingPlan
            modelBuilder.Entity<TrainingPlan>()
                        .Property(e => e.Id)
                        .ValueGeneratedOnAdd();  // Id is auto-generated on adding a new TrainingPlan

            // TrainingPlan to TrainingPlanPace relationship
            modelBuilder.Entity<TrainingPlanPace>()
                        .HasOne(tp => tp.TrainingPlan)
                        .WithMany(p => p.Paces)
                        .HasForeignKey(tp => tp.TrainingPlanId)
                        .OnDelete(DeleteBehavior.Cascade);  // Delete TrainingPlanPace when TrainingPlan is deleted
            modelBuilder.Entity<TrainingPlanPace>()
                        .HasIndex(tp => new { tp.TrainingPlanId, tp.WorkoutType, tp.PaceType })
                        .IsUnique();

            // Workout entity configuration
            modelBuilder.Entity<Workout>()
                        .HasKey(w => w.Id);  // Setting the primary key for Workout
            modelBuilder.Entity<Workout>()
                        .Property(w => w.Id)
                        .ValueGeneratedOnAdd();  // Id is auto-generated on adding a new Workout

            // Relationship configuration
            modelBuilder.Entity<Workout>()
                        .HasOne(w => w.TrainingPlan)  // Workout has one TrainingPlan
                        .WithMany(tp => tp.Workouts)  // TrainingPlan has many Workouts
                        .HasForeignKey(w => w.TrainingPlanId)  // ForeignKey in Workout entity is TrainingPlanId
                        .OnDelete(DeleteBehavior.Cascade);  // Delete Workout when TrainingPlan is deleted

            // RunningProfile entity configuration
            modelBuilder.Entity<RunningProfile>()
                        .HasKey(rp => rp.Id);  // Setting the primary key for RunningProfile
            modelBuilder.Entity<RunningProfile>()
                        .Property(rp => rp.Id)
                        .ValueGeneratedOnAdd();  // Id is auto-generated on adding a new RunningProfile

            // User entity configuration
            modelBuilder.Entity<User>()
                        .HasKey(u => u.Id);  // Setting the primary key for User
            modelBuilder.Entity<User>()
                        .Property(u => u.Id)
                        .ValueGeneratedOnAdd();  // Id is auto-generated on adding a new User
            modelBuilder.Entity<User>()
                        .HasOne(u => u.RunningProfile)  // User has one RunningProfile
                        .WithOne(rp => rp.User)  // RunningProfile has one User
                        .HasForeignKey<RunningProfile>(rp => rp.UserId)  // ForeignKey in RunningProfile entity is UserId
                        .OnDelete(DeleteBehavior.Cascade);  // Delete RunningProfile when User is deleted

        }
        public static DbContextOptionsBuilder<DatabaseContext> OptionsBuilder => new();
    }
}
