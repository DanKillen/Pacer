using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

// import the Entities (database models representing structure of tables in database)
using Pacer.Data.Entities;

namespace Pacer.Data.Repositories
{
    // The Context is How EntityFramework communicates with the database
    // We define DbSet properties for each table in the database
    public class DatabaseContext : DbContext
    {
        // authentication store
        public DbSet<User> Users { get; set; }
        public DbSet<ForgotPassword> ForgotPasswords { get; set; }
        public DbSet<RunningProfile> RunningProfiles { get; set; }
        public DbSet<TrainingPlan> TrainingPlans { get; set; }
        public DbSet<Workout> Workouts { get; set; }
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
                .HasForeignKey(w => w.TrainingPlanId);  // ForeignKey in Workout entity is TrainingPlanId
        }


        // Configure the context with logging - remove in production
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // remove in production 
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information).EnableSensitiveDataLogging();
        }

        public static DbContextOptionsBuilder<DatabaseContext> OptionsBuilder => new();

        // Convenience method to recreate the database thus ensuring the new database takes 
        // account of any changes to Models or DatabaseContext. ONLY to be used in development
        public void Initialise()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

    }
}
