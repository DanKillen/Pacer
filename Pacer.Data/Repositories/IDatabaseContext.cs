using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Pacer.Data.Entities;

namespace Pacer.Data.Repositories
{
    public interface IDatabaseContext
    {
        DbSet<User> Users { get; set; }
        DbSet<ForgotPassword> ForgotPasswords { get; set; }
        DbSet<RunningProfile> RunningProfiles { get; set; }
        DbSet<TrainingPlan> TrainingPlans { get; set; }
        DbSet<Workout> Workouts { get; set; }
        DbSet<TrainingPlanPace> Paces { get; set; }

        DatabaseFacade Database { get; }
        int SaveChanges();
        void SetEntityState<TEntity>(TEntity entity, EntityState state) where TEntity : class;

    }
}
