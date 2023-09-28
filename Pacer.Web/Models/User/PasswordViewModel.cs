using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Pacer.Web.Models.User;
public class PasswordViewModel
{
    public int Id { get; set; }

    [Required]
    [Remote(action: "VerifyPassword", controller: "User")]
    public string OldPassword { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage =
       "Password must be at least 8 characters long and contain at least 1 uppercase letter, 1 lowercase letter, 1 number, and 1 special character")]
    public string Password { get; set; }

    [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
    public string PasswordConfirm { get; set; }

}
