
using Pacer.Data.Entities;

namespace Pacer.Data.Services
{

    // This interface describes the operations that a UserService class implementation should provide
    public interface IUserService
    {
        // Initialise the repository - only to be used during development 
        //void Initialise();

        // ---------------- User Management --------------
        // Get a list of all users
        IList<User> GetUsers();
        // Get a list of all users, paged
        Paged<User> GetUsers(int page=1, int size=20, string orderBy="id", string direction="asc");
        // Get a single user by id
        User GetUser(int id);
        // Get a single user by email asynchronously
        Task<User> AsyncGetUserByEmail(string email);
        // Check if an email is available
        bool IsEmailAvailable(string email, int userId);
        // Add a new user asynchronously
        Task <User> AddUserAsync(string name, string email, string password);
        // Add a new user
        User AddUser(string name, string email, string password);
        // Verify a user's email address
        User VerifyEmail(int userId, string token);
        // Resend a verification token
        User ResendVerificationToken(string email);
        // Update a user
        User UpdateUser(User user);
        // Delete a user
        bool DeleteUser(int id);
        // ---------------- Authentication --------------
        // Authenticate a user
        Task<User> Authenticate(string email, string password);
        // ---------------- Password Reset --------------
        // send a password reset email
        string ForgotPassword(string email);
        // reset a user's password
        User ResetPassword(string email, string token, string password);
        // get a list of valid password reset tokens
        IList<string> GetValidPasswordResetTokens();
    }

}