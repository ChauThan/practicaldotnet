using MediatR;
using Microsoft.AspNetCore.Identity;
using App.Domain;

namespace App.Application.Feature.Roles
{
    public class RemoveRoleFromUser
    {
        public record Command(string UserId, string RoleName) : IRequest<Response>;
        public record Response(bool Succeeded, IEnumerable<string>? Errors = null);

        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly UserManager<ApplicationUser> _userManager;
            public Handler(UserManager<ApplicationUser> userManager)
            {
                _userManager = userManager;
            }
            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user == null)
                    return new Response(false, new[] { "User not found" });
                var result = await _userManager.RemoveFromRoleAsync(user, request.RoleName);
                return result.Succeeded ? new Response(true) : new Response(false, result.Errors.Select(e => e.Description));
            }
        }
    }
}
