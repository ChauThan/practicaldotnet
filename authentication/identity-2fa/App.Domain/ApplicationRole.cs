using Microsoft.AspNetCore.Identity;

namespace App.Domain;

public class ApplicationRole : IdentityRole<Guid>
{
    public ApplicationRole()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Initializes a new instance of <see cref="IdentityRole"/>.
    /// </summary>
    /// <param name="roleName">The role name.</param>
    /// <remarks>
    /// The Id property is initialized to form a new GUID string value.
    /// </remarks>
    public ApplicationRole(string roleName) : this()
    {
        Name = roleName;
    }
}