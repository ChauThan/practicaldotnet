using App.Application.Repositories;
using App.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace App.Application.Feature.Auth;

public static class Logout
{
    public class Command : IRequest<Response>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class Response
    {
        public bool Succeeded { get; set; }
        public string? Message { get; set; }
    }

    public class Handler(
        SignInManager<ApplicationUser> signInManager,
        IRefreshTokenRepository refreshTokenRepository)
        : IRequestHandler<Command, Response>
    {
        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var token = await refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
            if (token == null || token.IsRevoked || token.IsExpired)
            {
                return new Response { Succeeded = false, Message = "Invalid or already revoked refresh token." };
            }

            await signInManager.SignOutAsync();
            await refreshTokenRepository.RevokeTokenAsync(request.RefreshToken, cancellationToken);

            return new Response { Succeeded = true, Message = "Logged out and token revoked." };
        }
    }
}
