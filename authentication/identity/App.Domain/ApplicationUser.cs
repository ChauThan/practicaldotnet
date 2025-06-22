using Microsoft.AspNetCore.Identity;

namespace App.Domain;

internal class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
}
