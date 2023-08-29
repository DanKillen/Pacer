namespace Pacer.Data.Entities;

public class ForgotPassword
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Token { get; set; } = Guid.NewGuid().ToString();
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}

