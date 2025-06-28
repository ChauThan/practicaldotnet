using App.Application.Repositories;
using App.Application.Services;
using App.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace App.Application.Feature.Auth;

public static class RefreshToken
{
    public class Command : IRequest<Response>
    {
        public string RefreshToken { get; set; } = string.Empty;
        public string? AccessToken { get; set; } // Optional: for extra validation
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
