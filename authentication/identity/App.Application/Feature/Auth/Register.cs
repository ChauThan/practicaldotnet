using App.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace App.Application.Feature.Auth;

public static class Register
{
    public class Command : IRequest<Response>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }

    public class Response
    {
        public bool Succeeded { get; set; }
        public IEnumerable<string>? Errors { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
    }

    public class Handler(UserManager<ApplicationUser> userManager) 
        : IRequestHandler<Command, Response>
    {
        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateCreated = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                return new Response
                {
                    Succeeded = true,
                    UserId = user.Id,
                    UserName = user.UserName
                };
            }

            return new Response
            {
                Succeeded = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }
    }
}