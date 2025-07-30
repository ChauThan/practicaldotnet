using App.Domain;
using App.Application.Abstractions;
using MediatR;
using System.Security.Claims;

namespace App.Application.Feature.Auth;

public static class LoginWith2FA
{
    public class Command : IRequest<Response>
    {
        public Guid UserId { get; set; }
        public string Code { get; set; } = string.Empty;
        public bool RememberMe { get; set; } = false;
        public bool RememberClient { get; set; } = false;
    }

    public class Response
    {
        public bool Succeeded { get; set; }
        public string? Message { get; set; }
        public Guid? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Token { get; set; }
    }

    public class Handler(
        IUserService userService,
        ISignInService signInService,
        IJwtService jwtService)
        : IRequestHandler<Command, Response>
    {
        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await userService.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return new Response { Succeeded = false, Message = "User not found." };
            }

            // Check if user has 2FA enabled
            var is2FAEnabled = await userService.GetTwoFactorEnabledAsync(user);
            if (!is2FAEnabled)
            {
                return new Response { Succeeded = false, Message = "Two-factor authentication is not enabled for this user." };
            }

            var result = await signInService.TwoFactorAuthenticatorSignInAsync(user, request.Code, request.RememberMe, request.RememberClient);

            if (result.Succeeded)
            {
                var claims = new List<Claim>
                {
                    new("sub", user.Id.ToString()),
                    new("jti", Guid.NewGuid().ToString()),
                    new("email", user.Email!),
                    new("name", user.UserName!)
                };

                var token = jwtService.GenerateToken(user, claims);

                return new Response
                {
                    Succeeded = true,
                    Message = "Login successful.",
                    UserId = user.Id,
                    UserName = user.UserName,
                    Token = token
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
                return new Response { Succeeded = false, Message = "Invalid two-factor code." };
            }
        }
    }
}
