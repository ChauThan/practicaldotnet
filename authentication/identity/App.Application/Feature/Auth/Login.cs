using App.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;

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
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Token { get; set; }
    }

    // The IJwtService dependency is commented out below because we want to focus on the basic identity functionality.
    // JWT token generation can be added later when authentication logic is extended.
    public class Handler(
        UserManager<ApplicationUser> userManager, 
        SignInManager<ApplicationUser> signInManager /*, IJwtService jwtService */) 
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
                
                // var token = _jwtService.GenerateToken(user);
                return new Response
                {
                    Succeeded = true,
                    Message = "Login successful.",
                    UserId = user.Id,
                    UserName = user.UserName,
                    Token = "YOUR_JWT_TOKEN_HERE"
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