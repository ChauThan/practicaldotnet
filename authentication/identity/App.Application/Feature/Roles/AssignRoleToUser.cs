using App.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace App.Application.Feature.Roles;
public static class AssignRoleToUser
{
    public class Command : IRequest<Response>
    {
        public string UserId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
    }

    public class Response
    {
        public bool Succeeded { get; set; }
        public IEnumerable<string>? Errors { get; set; }
        public Guid? UserId { get; set; }
        public string? UserName { get; set; }
        public string? RoleName { get; set; }
    }

    public class Handler(
        UserManager<ApplicationUser> userManager, 
        RoleManager<ApplicationRole> roleManager) 
        : IRequestHandler<Command, Response>
    {
        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return new Response { Succeeded = false, Errors = [$"User with ID '{request.UserId}' not found."] };
            }

            var roleExists = await roleManager.RoleExistsAsync(request.RoleName);
            if (!roleExists)
            {
                return new Response { Succeeded = false, Errors = [$"Role '{request.RoleName}' does not exist."] };
            }

            var result = await userManager.AddToRoleAsync(user, request.RoleName);

            if (result.Succeeded)
            {
                return new Response
                {
                    Succeeded = true,
                    UserId = user.Id,
                    UserName = user.UserName,
                    RoleName = request.RoleName
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