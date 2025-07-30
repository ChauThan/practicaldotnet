using App.Application.Abstractions;
using MediatR;

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

    public class Handler(IRoleService roleService)
        : IRequestHandler<Command, Response>
    {
        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var roleExists = await roleService.RoleExistsAsync(request.RoleName);
            if (roleExists)
            {
                return new Response { Succeeded = false, Errors = [$"Role '{request.RoleName}' already exists."] };
            }

            var (succeeded, errors, roleId) = await roleService.CreateRoleAsync(request.RoleName);

            if (succeeded)
            {
                return new Response
                {
                    Succeeded = true,
                    RoleId = roleId,
                    RoleName = request.RoleName
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