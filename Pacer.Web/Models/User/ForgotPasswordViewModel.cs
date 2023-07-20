using System.ComponentModel.DataAnnotations;

namespace Pacer.Web.Models.User;
public class ForgotPasswordViewModel
{
    [Required]
    public string Email { get; set; }
    
}
