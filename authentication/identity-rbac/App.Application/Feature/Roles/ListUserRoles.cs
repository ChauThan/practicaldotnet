using MediatR;
using Microsoft.AspNetCore.Identity;
using App.Domain;

namespace App.Application.Feature.Roles
{
    public class ListUserRoles
    {
        public record Query(string UserId) : IRequest<List<string>>;

        public class Handler : IRequestHandler<Query, List<string>>
        {
            private readonly UserManager<ApplicationUser> _userManager;
            public Handler(UserManager<ApplicationUser> userManager)
            {
                _userManager = userManager;
            }
            public async Task<List<string>> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user == null)
                    return new List<string>();
                var roles = await _userManager.GetRolesAsync(user);
                return roles.ToList();
            }
        }
    }
}
