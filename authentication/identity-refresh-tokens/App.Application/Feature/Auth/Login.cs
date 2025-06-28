using App.Application.Repositories;
using App.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        public DateTime AccessTokenExpiration { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }

    // The IJwtService dependency is commented out below because we want to focus on the basic identity functionality.
    // JWT token generation can be added later when authentication logic is extended.
    public class Handler(
        UserManager<ApplicationUser> userManager, 
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration,
        IRefreshTokenRepository refreshTokenRepository)
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
                var (accessToken, expiration, jwtId) = GenerateJwtToken(user); // Cần jwtId để liên kết với Refresh Token
                var refreshToken = GenerateRefreshToken(user, jwtId);

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

        private (string AccessToken, DateTime Expiration, string JwtId) GenerateJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email!),
                new(ClaimTypes.Name, user.UserName!)
            };

            var roles = userManager.GetRolesAsync(user).Result;
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var accessTokenLifeTimeMinutes = Convert.ToDouble(configuration["JwtSettings:AccessTokenLifeTimeMinutes"] ?? "1");
            var expiration = DateTime.UtcNow.AddMinutes(accessTokenLifeTimeMinutes);

            var token = new JwtSecurityToken(
                issuer: configuration["JwtSettings:Issuer"],
                audience: configuration["JwtSettings:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            var jwtId = token.Id;

            return (new JwtSecurityTokenHandler().WriteToken(token), expiration, jwtId);
        }

        private RefreshToken GenerateRefreshToken(ApplicationUser user, string jwtId)
        {
            var refreshTokenLifeTimeDays = Convert.ToDouble(configuration["JwtSettings:RefreshTokenLifeTimeDays"] ?? "3");

            return new RefreshToken
            {
                Id = Guid.NewGuid(),
                JwtId = jwtId,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(refreshTokenLifeTimeDays),
                Token = Guid.NewGuid().ToString() + Guid.NewGuid().ToString()
            };
        }
    }
}