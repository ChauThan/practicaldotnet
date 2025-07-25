using App.Domain;
using App.Application.Abstractions;
using MediatR;
using System.Security.Claims;

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
        public string? Token { get; set; }
    }

    // The IJwtService dependency is now properly defined and will be implemented in Infrastructure
    public class Handler(
        IUserService userService,
        ISignInService signInService,
        IJwtService jwtService)
        : IRequestHandler<Query, Response>
    {
        public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            var user = await userService.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return new Response { Succeeded = false, Message = "Invalid credentials." };
            }

            var result = await signInService.CheckPasswordSignInAsync(user, request.Password, false);

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
                return new Response { Succeeded = false, Message = "Invalid credentials." };
            }
        }
    }
}