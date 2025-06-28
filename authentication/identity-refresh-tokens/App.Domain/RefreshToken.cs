namespace App.Domain;

public class RefreshToken
{
    public Guid Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public string JwtId { get; set; } = string.Empty; 

    public DateTime CreationDate { get; set; }
    public DateTime ExpiryDate { get; set; }

    public DateTime? RevokedDate { get; set; }
    public bool IsRevoked => RevokedDate != null;

    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public bool IsExpired => DateTime.UtcNow >= ExpiryDate; 
    public bool IsActive => !IsRevoked && !IsExpired;
}