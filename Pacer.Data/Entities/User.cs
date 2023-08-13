using System;

namespace Pacer.Data.Entities;
// Add User roles relevant to your application
public enum Role { admin, manager, guest }

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    
    //Email verification
    public bool EmailVerified { get; set; } = false;
    public string EmailVerificationToken { get; set; }
    public string Password { get; set; }

    // User role within application
    public Role Role { get; set; }

    public int RunningProfileId { get; set; }
    public RunningProfile RunningProfile { get; set; }

}

