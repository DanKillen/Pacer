
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

using Pacer.Data.Entities;
using Pacer.Data.Services;
using Microsoft.AspNetCore.Authorization;
using Pacer.Data.Security;
using Pacer.Web.Models.User;
using System.Security.Claims;

/**
 *  User Management Controller
 */
namespace Pacer.Web.Controllers
{
    public class UserController : BaseController
    {
        private readonly IConfiguration _config;
        private readonly IMailService _mailer;
        private readonly IUserService _svc;

        public UserController(IUserService svc, IConfiguration config, IMailService mailer, ILogger<UserController> logger) : base(logger)
        {
            _config = config;
            _mailer = mailer;
            _svc = svc;
        }

        // HTTP GET - Display Paged List of Users
        [Authorize]
        public ActionResult Index(int page = 1, int size = 20, string order = "id", string direction = "asc")
        {
            if (User.HasClaim(ClaimTypes.Role, "admin") || User.HasClaim(ClaimTypes.Role, "manager"))
            {
                var paged = _svc.GetUsers(page, size, order, direction);
                return View(paged);
            }
            Alert("You do not have permission to view this page.", AlertType.warning);
            return RedirectToAction("Index", "Home");

        }

        // HTTP GET - Display Login page
        public IActionResult Login()
        {
            return View();
        }

        // HTTP POST - Login action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Email,Password")] LoginViewModel m)
        {
            var user = await _svc.Authenticate(m.Email, m.Password);
            // check if login was unsuccessful and add validation errors
            if (user == null)
            {
                ModelState.AddModelError("Email", "Invalid Login Credentials");
                ModelState.AddModelError("Password", "Invalid Login Credentials");
                return View(m);
            }
            if (!user.EmailVerified)
            {
                Alert("Please verify your email before logging in.", AlertType.warning);
                return View(m);
            }

            // Sign user in using cookie authentication
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, AuthBuilder.BuildClaimsPrincipal(user));
            // No Alert as it will redirect to a welcome page
            return Redirect("/");
        }


        // HTTP GET - Display Register page
        public IActionResult Register()
        {
            return View();
        }

        // HTTP POST - Register action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Name,Email,Password,PasswordConfirm,Role")] RegisterViewModel m)
        {
            if (!ModelState.IsValid)
            {
                return View(m);
            }
            // add user via service
            var user = await _svc.AddUserAsync(m.Name, m.Email, m.Password);

            // check if error adding user and display warning
            if (user == null)
            {
                Alert("There was a problem Registering. Please try again", AlertType.warning);
                return View(m);
            }
            var verificationUrl = $"{Request.Scheme}://{Request.Host}/User/VerifyEmail?userId={user.Id}&token={user.EmailVerificationToken}";
            var emailSubject = "Email Verification for Pacer";
            var emailMessage = @$" 
                                <h3>Email Verification for Pacer</h3>
                                <p>Please click the link below to verify your email address:</p>
                                <a href='{verificationUrl}'>
                                {verificationUrl}</a>. 
                                <p>If you didn't request this email, please ignore it.</p>
                                ";
            _mailer.SendMail(emailSubject, emailMessage, user.Email);


            Alert("Successfully Registered. Please verify your email by clicking the link in the email we have sent to you.", AlertType.info);
            _logger.LogInformation($"User {user.Id} successfully registered @ {DateTime.UtcNow}");
            return RedirectToAction(nameof(Login));
        }
        [HttpGet]
        public IActionResult VerifyEmail(int userId, string token)
        {
            var user = _svc.VerifyEmail(userId, token);
            if (user != null)
            {
                Alert("Email verified successfully! You can now log in.", AlertType.success);
                return RedirectToAction(nameof(Login));
            }
            Alert("Invalid verification token, have you requested a new token since?", AlertType.warning);
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult ResendVerificationEmail()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResendVerificationToken(string email)
        {
            var user = _svc.ResendVerificationToken(email);

            if (user == null)
            {
                Alert("Either the email doesn't exist or it has already been verified.", AlertType.warning);
                return View();
            }

            var verificationUrl = $"{Request.Scheme}://{Request.Host}/User/VerifyEmail?userId={user.Id}&token={user.EmailVerificationToken}";
            var emailSubject = "Email Verification for Pacer - Resend";
            var emailMessage = @$" 
                                    <h3>Email Verification for Pacer</h3>
                                    <p>Please click the link below to verify your email address:</p>
                                    <a href='{verificationUrl}'>
                                    {verificationUrl}</a>. 
                                    <p>If you didn't request this email, please ignore it.</p>
                                ";
            _mailer.SendMail(emailSubject, emailMessage, user.Email);

            Alert("Verification email resent. Please check your inbox.", AlertType.info);
            return RedirectToAction(nameof(Login));
        }

        // HTTP GET - Display Update profile page
        [Authorize]
        public IActionResult UpdateProfile()
        {
            // use BaseClass helper method to retrieve Id of signed in user 
            var user = _svc.GetUser(User.GetSignedInUserId());
            var profileViewModel = new ProfileViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };
            return View(profileViewModel);
        }

        // HTTP POST - Update profile action
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile([Bind("Id,Name,Email")] ProfileViewModel m)
        {
            var user = _svc.GetUser(m.Id);
            // check if form is invalid and redisplay
            if (!ModelState.IsValid || user == null)
            {
                return View(m);
            }

            // update user details and call service
            user.Name = m.Name;
            user.Email = m.Email;
            var updated = _svc.UpdateUser(user);

            // check if error updating service
            if (updated == null)
            {
                Alert("There was a problem Updating. Please try again", AlertType.warning);
                return View(m);
            }

            Alert("Successfully Updated Account Details", AlertType.info);

            // sign the user in with updated details)
            await SignInCookieAsync(user);

            return RedirectToAction("Index", "Home");
        }

        // HTTP GET - Allow admin to update a User
        [Authorize(Roles = "admin")]
        public IActionResult Update(int id)
        {
            // retrieve user 
            var user = _svc.GetUser(id);
            var profileViewModel = new ProfileViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };
            return View(profileViewModel);
        }

        // HTTP POST - Update User action
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update([Bind("Id,Name,Email,Role")] ProfileViewModel m)
        {
            var user = _svc.GetUser(m.Id);
            // check if form is invalid and redisplay
            if (!ModelState.IsValid || user == null)
            {
                return View(m);
            }

            // update user details and call service
            user.Name = m.Name;
            user.Email = m.Email;
            user.Role = m.Role;
            var updated = _svc.UpdateUser(user);

            // check if error updating service
            if (updated == null)
            {
                Alert("There was a problem Updating. Please try again", AlertType.warning);
                return View(m);
            }

            Alert("Successfully Updated User Account Details", AlertType.info);
            _logger.LogInformation($"User {user.Id} successfully updated @ {DateTime.UtcNow} by {User.GetSignedInUserId()}");
            return RedirectToAction("Index", "User");
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var user = _svc.GetUser(id);
            if(user == null)
            {
                Alert("User not found", AlertType.warning);
                return RedirectToAction("Index", "User");
            }
            return View(new DeleteViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            });
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var deleted = _svc.DeleteUser(id);
            if(!deleted)
            {
                Alert("There was a problem deleting this user. Maybe they were already deleted", AlertType.warning);
            }
            else
            {
                Alert("User deleted successfully", AlertType.success);
            }
            return RedirectToAction("Index","User");
        }
        
        // HTTP GET - Display update password page
        [Authorize]
        public IActionResult UpdatePassword()
        {
            // use BaseClass helper method to retrieve Id of signed in user 
            var user = _svc.GetUser(User.GetSignedInUserId());
            var passwordViewModel = new PasswordViewModel
            {
                Id = user.Id,
                Password = user.Password,
                PasswordConfirm = user.Password,
            };
            return View(passwordViewModel);
        }

        // HTTP POST - Update Password action
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword([Bind("Id,OldPassword,Password,PasswordConfirm")] PasswordViewModel m)
        {
            var user = _svc.GetUser(m.Id);
            if (!ModelState.IsValid || user == null)
            {
                return View(m);
            }
            // update the password
            user.Password = m.Password;
            // save changes      
            var updated = _svc.UpdateUser(user);
            if (updated == null)
            {
                Alert("There was a problem Updating the password. Please try again", AlertType.warning);
                return View(m);
            }

            Alert("Successfully Updated Password", AlertType.info);
            // sign the user in with updated details
            await SignInCookieAsync(user);

            return RedirectToAction("Index", "Home");
        }

        // HTTP POST - Logout action
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }


        // HTTP GET - Display Forgot password page
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // HTTP POST - Forgot password action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword([Bind("Email")] ForgotPasswordViewModel m)
        {
            var token = _svc.ForgotPassword(m.Email);
            if (token == null)
            {
                // No such account. Alert only for testing
                Alert("No account found", AlertType.warning);
                return RedirectToAction(nameof(Login));
            }

            // build reset password url and email html message
            var url = $"{Request.Scheme}://{Request.Host}/User/ResetPassword?token={token}&email={m.Email}";
            var message = @$" 
                <h3>Password Reset</h3>
                <a href='{url}'>
                   {url}
                </a>
            ";

            // send email containing reset token
            if (!_mailer.SendMail("Password Reset Request", message, m.Email))
            {
                Alert("There was a problem sending a password reset email", AlertType.warning);

                return RedirectToAction(nameof(ForgotPassword));
            }

            Alert("Password Reset Token sent to your registered email account", AlertType.info);
            return RedirectToAction(nameof(PasswordResetSent));
        }

        public IActionResult PasswordResetSent()
        {
            return View();
        }


        // HTTP GET - Display Reset password page
        public IActionResult ResetPassword(string token = null, string email = null)
        {
            var model = new ResetPasswordViewModel
            {
                Token = token,
                Email = email
            };
            return View(model);
        }


        // HTTP POST - ResetPassword action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword([Bind("Email,Password,Token")] ResetPasswordViewModel m)
        {
            // verify reset request
            var user = _svc.ResetPassword(m.Email, m.Token, m.Password);
            if (user == null)
            {
                Alert("Invalid Password Reset Request", AlertType.warning);
                return RedirectToAction(nameof(ResetPassword));
            }

            Alert("Password reset successfully", AlertType.success);
            _logger.LogInformation($"User {user.Email} reset password successfully");
            return RedirectToAction(nameof(Login));
        }

        // HTTP GET - Display not authorised and not authenticated pages
        public IActionResult ErrorNotAuthorised() => View();
        public IActionResult ErrorNotAuthenticated() => View();


        // -------------------------- Helper Methods ------------------------------

        // Called by Remote Validation attribute on RegisterViewModel to verify email address is available
        [AcceptVerbs("GET", "POST")]
        public IActionResult VerifyEmailAvailable(string email, int id)
        {
            // check if email is available, or owned by user with id 
            if (!_svc.IsEmailAvailable(email, id))
            {
                return Json($"A user with this email address {email} already exists.");
            }
            return Json(true);
        }

        // Called by Remote Validation attribute on ChangePassword to verify old password
        [AcceptVerbs("GET", "POST")]
        public IActionResult VerifyPassword(string oldPassword)
        {
            // use BaseClass helper method to retrieve Id of signed in user 
            var id = User.GetSignedInUserId();
            // check if email is available, unless already owned by user with id
            var user = _svc.GetUser(id);
            if (user == null || !Hasher.ValidateHash(user.Password, oldPassword))
            {
                return Json($"Please enter current password.");
            }
            return Json(true);
        }

        // Sign user in using Cookie authentication scheme
        private async Task SignInCookieAsync(User user)
        {
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                AuthBuilder.BuildClaimsPrincipal(user)
            );
        }
    }
}