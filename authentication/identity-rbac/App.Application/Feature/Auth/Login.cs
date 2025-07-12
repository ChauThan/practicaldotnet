using App.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

    // The IJwtService dependency is commented out below because we want to focus on the basic identity functionality.
    // JWT token generation can be added later when authentication logic is extended.
    public class Handler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration /*, IJwtService jwtService */)
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
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                    new Claim(ClaimTypes.Name, user.UserName!)
                };

                var roles = await userManager.GetRolesAsync(user);
                claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]!));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var expires = DateTime.Now.AddDays(Convert.ToDouble(configuration["JwtSettings:ExpireDays"] ?? "7"));

                var token = new JwtSecurityToken(
                    issuer: configuration["JwtSettings:Issuer"],
                    audience: configuration["JwtSettings:Audience"],
                    claims: claims,
                    expires: expires,
                    signingCredentials: creds
                );

                // var token = _jwtService.GenerateToken(user);
                return new Response
                {
                    Succeeded = true,
                    Message = "Login successful.",
                    UserId = user.Id,
                    UserName = user.UserName,
                    Token = new JwtSecurityTokenHandler().WriteToken(token)
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