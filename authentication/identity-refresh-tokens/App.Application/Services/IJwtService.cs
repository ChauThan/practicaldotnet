namespace App.Application.Services;

using App.Domain;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

/// <summary>
/// Provides methods for generating JWT access tokens and refresh tokens for authentication.
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Generates a JWT access token for the specified user.
    /// </summary>
    /// <param name="user">The application user for whom to generate the token.</param>
    /// <returns>A tuple containing the access token string, its expiration time, and the JWT ID.</returns>
    (string AccessToken, DateTime Expiration, string JwtId) GenerateJwtToken(ApplicationUser user);

    /// <summary>
    /// Generates a refresh token for the specified user and JWT ID.
    /// </summary>
    /// <param name="user">The application user for whom to generate the refresh token.</param>
    /// <param name="jwtId">The JWT ID to associate with the refresh token.</param>
    /// <returns>A new <see cref="RefreshToken"/> instance.</returns>
    RefreshToken GenerateRefreshToken(ApplicationUser user, string jwtId);

    /// <summary>
    /// Validates the JWT access token and returns the ClaimsPrincipal.
    /// Throws if invalid or malformed.
    /// </summary>
    ClaimsPrincipal ValidateToken(string accessToken);

    /// <summary>
    /// Extracts the subject (sub) and JWT ID (jti) claims from a ClaimsPrincipal.
    /// Throws if missing.
    /// </summary>
    (string Sub, string Jti) ExtractSubAndJti(ClaimsPrincipal principal);
}
