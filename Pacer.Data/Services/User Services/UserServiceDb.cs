
using Pacer.Data.Entities;
using Pacer.Data.Services;
using Pacer.Data.Security;
using Pacer.Data.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Pacer.Data.Services
{
    public class UserServiceDb : IUserService
    {
        private readonly IDatabaseContext _ctx;

        public UserServiceDb(IDatabaseContext ctx)
        {
            _ctx = ctx;
        }


        // ------------------ User Related Operations ------------------------

        // retrieve list of Users
        public IList<User> GetUsers()
        {
            return _ctx.Users.ToList();
        }

        // retrieve paged list of users
        public Paged<User> GetUsers(int page = 1, int size = 10, string orderBy = "id", string direction = "asc")
        {
            var query = (orderBy.ToLower(), direction.ToLower()) switch
            {
                ("id", "asc") => _ctx.Users.OrderBy(r => r.Id),
                ("id", "desc") => _ctx.Users.OrderByDescending(r => r.Id),
                ("name", "asc") => _ctx.Users.OrderBy(r => r.Name),
                ("name", "desc") => _ctx.Users.OrderByDescending(r => r.Name),
                ("email", "asc") => _ctx.Users.OrderBy(r => r.Email),
                ("email", "desc") => _ctx.Users.OrderByDescending(r => r.Email),
                _ => _ctx.Users.OrderBy(r => r.Id)
            };

            return query.ToPaged(page, size, orderBy, direction);
        }

        // Retrive User by Id 
        public User GetUser(int id)
        {
            return _ctx.Users.FirstOrDefault(s => s.Id == id);
        }

        // Add a new User checking a User with same email does not exist
        public async Task<User> AddUserAsync(string name, string email, string password)
        {
            var existing = await AsyncGetUserByEmail(email);
            if (existing != null)
            {
                return null;
            }

            // Capitalize the first letter of the name if it's a letter
            if (char.IsLetter(name[0]))
            {
                name = char.ToUpper(name[0]) + name[1..];
            }          
            // Email verifications are currently disabled
            // var user = new User
            // {
            //     Name = name,
            //     Email = email,
            //     EmailVerified = false,
            //     EmailVerificationToken = Guid.NewGuid().ToString(),
            //     Password = Hasher.CalculateHash(password),
            //     Role = Role.guest
            // };
            var user = new User
            {
                Name = name,
                Email = email,
                EmailVerified = true,
                EmailVerificationToken = null,
                Password = Hasher.CalculateHash(password),
                Role = Role.guest
            };
            _ctx.Users.Add(user);
            _ctx.SaveChanges();
            return user; // return newly added User
        }

        public User AddUser(string name, string email, string password)
        {
            var existing = GetUserByEmail(email);
            if (existing != null)
            {
                return null;
            }

            // Capitalize the first letter of the name if it's a letter
            if (char.IsLetter(name[0]))
            {
                name = char.ToUpper(name[0]) + name[1..];
            }          
            // Email verifications are currently disabled
            // var user = new User
            // {
            //     Name = name,
            //     Email = email,
            //     EmailVerified = false,
            //     EmailVerificationToken = Guid.NewGuid().ToString(),
            //     Password = Hasher.CalculateHash(password),
            //     Role = Role.guest
            // };
            var user = new User
            {
                Name = name,
                Email = email,
                EmailVerified = true,
                EmailVerificationToken = null,
                Password = Hasher.CalculateHash(password),
                Role = Role.guest
            };
            _ctx.Users.Add(user);
            _ctx.SaveChanges();
            return user; // return newly added User
        }
        // Verify the email address of user
        public User VerifyEmail(int userId, string token)
        {
            var user = _ctx.Users.FirstOrDefault(u => u.Id == userId && u.EmailVerificationToken == token);
            if (user != null)
            {
                user.EmailVerified = true;
                user.EmailVerificationToken = null; // Clear the token after verification
                _ctx.SaveChanges();
                return user;
            }
            return null;
        }
        public User ResendVerificationToken(string email)
        {
            var user = _ctx.Users.FirstOrDefault(u => u.Email == email && !u.EmailVerified);
            if (user == null)
            {
                return null; // Either user doesn't exist or has already verified email.
            }

            user.EmailVerificationToken = Guid.NewGuid().ToString();
            _ctx.SaveChanges();
            return user;
        }

        // Delete the User identified by Id returning true if deleted and false if not found
        public bool DeleteUser(int id)
        {
            var s = GetUser(id);
            if (s == null)
            {
                return false;
            }
            _ctx.Users.Remove(s);
            _ctx.SaveChanges();
            return true;
        }

        // Update the User with the details in updated 
        public User UpdateUser(User updated)
        {
            // verify the User exists
            var User = GetUser(updated.Id);
            if (User == null)
            {
                return null;
            }
            // verify email address is registered or available to this user
            if (!IsEmailAvailable(updated.Email, updated.Id))
            {
                return null;
            }
            // update the details of the User retrieved and save
            User.Name = updated.Name;
            User.Email = updated.Email;
            User.Password = Hasher.CalculateHash(updated.Password);
            User.Role = updated.Role;


            _ctx.SaveChanges();
            return User;
        }

        // Find a user with specified email address
        public async Task<User> AsyncGetUserByEmail(string email)
        {
            return await _ctx.Users
                            .Include(u => u.RunningProfile)
                            .FirstOrDefaultAsync(u => u.Email == email);
        }
        public User GetUserByEmail(string email)
        {
            return _ctx.Users
                       .Include(u => u.RunningProfile)
                       .FirstOrDefault(u => u.Email == email);
        }

        // Verify if email is available or registered to specified user
        public bool IsEmailAvailable(string email, int userId)
        {
            return _ctx.Users.FirstOrDefault(u => u.Email == email && u.Id != userId) == null;
        }

        public IList<User> GetUsersQuery(Func<User, bool> q)
        {
            return _ctx.Users.Where(q).ToList();
        }

        public async Task<User> Authenticate(string email, string password)
        {
            // retrieve the user based on the EmailAddress (assumes EmailAddress is unique)
            var user = await AsyncGetUserByEmail(email);

            // Verify the user exists and Hashed User password matches the password provided
            return (user != null && Hasher.ValidateHash(user.Password, password)) ? user : null;

            //return (user != null && user.Password == password ) ? user: null;
        }

        public string ForgotPassword(string email)
        {
            var user = _ctx.Users.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
                // invalidate any previous tokens
                _ctx.ForgotPasswords
                    .Where(t => t.Email == email && t.ExpiresAt > DateTime.Now).ToList()
                    .ForEach(t => t.ExpiresAt = DateTime.Now);

                var f = new ForgotPassword
                {
                    Email = email,
                    CreatedAt = DateTime.Now, // Setting the current time to CreatedAt
                    ExpiresAt = DateTime.Now.AddHours(24) // Setting the token to expire in 24 hours
                };
                _ctx.ForgotPasswords.Add(f);
                _ctx.SaveChanges();
                return f.Token;
            }
            return null;
        }

        public User ResetPassword(string email, string token, string password)
        {
            // find user by email
            var user = _ctx.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return null; // user not found
            }
            // find valid reset token for user
            var reset = _ctx.ForgotPasswords
                           .FirstOrDefault(t => t.Email == email && t.Token == token && t.ExpiresAt > DateTime.Now);
            if (reset == null)
            {
                return null; // reset token invalid
            }

            // valid token and user so update password, invalidate the token and return the user           
            reset.ExpiresAt = DateTime.Now;
            user.Password = Hasher.CalculateHash(password);
            _ctx.SaveChanges();
            return user;
        }

        public IList<string> GetValidPasswordResetTokens()
        {
            // return non expired tokens
            return _ctx.ForgotPasswords.Where(t => t.ExpiresAt > DateTime.Now)
                                      .Select(t => t.Token)
                                      .ToList();
        }

    }
}