using App.Domain;
using App.Application.Abstractions;
using MediatR;

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
        public Guid? UserId { get; set; }
        public string? UserName { get; set; }
    }

    public class Handler(IUserService userService)
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

            var (succeeded, errors, userId) = await userService.CreateUserAsync(user, request.Password);

            if (succeeded)
            {
                return new Response
                {
                    Succeeded = true,
                    UserId = userId,
                    UserName = user.UserName
                };
            }

            return new Response
            {
                Succeeded = false,
                Errors = errors
            };
        }
    }
}