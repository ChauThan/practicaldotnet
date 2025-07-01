using App.Application.Repositories;
using App.Application.Services;
using App.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace App.Application.Feature.Auth;

public static class Login
{
    public class Query : IRequest<Response>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
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
        SignInManager<ApplicationUser> signInManager,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtService jwtService)
        : IRequestHandler<Query, Response>
    {
        public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return new Response { Succeeded = false, Message = "Invalid credentials." };
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (result.Succeeded)
            {
                var (accessToken, expiration, jwtId) = jwtService.GenerateJwtToken(user);
                var refreshToken = jwtService.GenerateRefreshToken(user, jwtId);

                await refreshTokenRepository.AddAsync(refreshToken, CancellationToken.None);

                return new Response
                {
                    Succeeded = true,
                    Message = "Login successful.",
                    UserId = user.Id,
                    UserName = user.UserName,
                    AccessToken = accessToken,
                    AccessTokenExpiration = expiration,
                    RefreshToken = refreshToken.Token,
                };
            }
            else if (result.IsLockedOut)
            {
                return new Response { Succeeded = false, Message = "User account locked out." };
            }
            else if (result.IsNotAllowed)
            {
                return new Response { Succeeded = false, Message = "User not allowed to login." };
            }
            else
            {
                return new Response { Succeeded = false, Message = "Invalid credentials." };
            }
        }
    }
}