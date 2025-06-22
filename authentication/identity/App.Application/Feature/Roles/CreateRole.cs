using App.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace App.Application.Feature.Roles;

public static class CreateRole
{
    public class Command : IRequest<Response>
    {
        public string RoleName { get; set; } = string.Empty;
    }

    public class Response
    {
        public bool Succeeded { get; set; }
        public IEnumerable<string>? Errors { get; set; }
        public Guid? RoleId { get; set; }
        public string? RoleName { get; set; }
    }

    public class Handler(RoleManager<ApplicationRole> roleManager) 
        : IRequestHandler<Command, Response>
    {
        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var roleExists = await roleManager.RoleExistsAsync(request.RoleName);
            if (roleExists)
            {
                return new Response { Succeeded = false, Errors = [$"Role '{request.RoleName}' already exists."] };
            }

            var role = new ApplicationRole(request.RoleName);
            var result = await roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                return new Response
                {
                    Succeeded = true,
                    RoleId = role.Id,
                    RoleName = role.Name
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