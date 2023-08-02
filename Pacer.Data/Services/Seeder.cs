
using Pacer.Data.Entities;

namespace Pacer.Data.Services
{
    public static class Seeder
    {
        // use this class to seed the database with dummy test data using an IUserService 
        public static void Seed(IUserService svc, IRunningProfileService runningProfileService, ITrainingPlanService trainingPlanService)
        {

            // seeder destroys and recreates the database - NOT to be called in production!!!
            svc.Initialise();

            // add users
            svc.AddUser("Administrator", "admin@mail.com", "admin", Role.admin);
            svc.AddUser("Manager", "manager@mail.com", "manager", Role.manager);
            svc.AddUser("Guest", "guest@mail.com", "guest", Role.guest);

            runningProfileService.CreateProfile(1, new DateTime(1999, 3, 3), "Male", 5, new TimeSpan(0, 20, 0));

            trainingPlanService.CreatePlan(1, RaceType.Marathon, new DateTime(2023, 10, 08), new TimeSpan(3, 30, 0));

 
        }
    }

}