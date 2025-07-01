using App.Application.Repositories;
using App.Application.Services;
using App.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace App.Application.Feature.Auth;

public static class RefreshToken
{
    public class Command : IRequest<Response>
    {
        public string RefreshToken { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
    }

    public class Response
    {
        public bool Succeeded { get; set; }
        public string? Message { get; set; }
        public Guid? UserId { get; set; }
        public string? UserName { get; set; }
        public DateTime AccessTokenExpiration { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class Handler(
        UserManager<ApplicationUser> userManager,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtService jwtService)
        : IRequestHandler<Command, Response>
    {
        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var storedToken = await refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
            if (storedToken == null || !storedToken.IsActive)
            {
                return new Response { Succeeded = false, Message = "Invalid or expired refresh token." };
            }

            var user = await userManager.FindByIdAsync(storedToken.UserId.ToString());
            if (user == null)
            {
                return new Response { Succeeded = false, Message = "User not found." };
            }

            // Validate AccessToken using JwtService
            try
            {
                var principal = jwtService.ValidateToken(request.AccessToken);
                var (sub, jti) = jwtService.ExtractSubAndJti(principal);
                if (sub != user.Id.ToString() || jti != storedToken.JwtId)
                {
                    return new Response { Succeeded = false, Message = "Access token does not match refresh token." };
                }
            }
            catch
            {
                return new Response { Succeeded = false, Message = "Invalid access token format." };
            }

            // Revoke the old refresh token
            storedToken.RevokedDate = DateTime.UtcNow;
            await refreshTokenRepository.UpdateAsync(storedToken, cancellationToken);

            // Generate new tokens
            var (accessToken, expiration, jwtId) = jwtService.GenerateJwtToken(user);
            var newRefreshToken = jwtService.GenerateRefreshToken(user, jwtId);
            await refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);

            return new Response
            {
                Succeeded = true,
                Message = "Token refreshed.",
                UserId = user.Id,
                UserName = user.UserName,
                AccessToken = accessToken,
                AccessTokenExpiration = expiration,
                RefreshToken = newRefreshToken.Token
            };
        }
    }
}
