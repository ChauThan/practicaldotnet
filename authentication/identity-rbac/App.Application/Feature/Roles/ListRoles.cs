
using MediatR;
using Microsoft.AspNetCore.Identity;
using App.Domain;

namespace App.Application.Feature.Roles
{
    public class ListRoles
    {
        public record Query : IRequest<List<string>>;

        public class Handler : IRequestHandler<Query, List<string>>
        {
            private readonly RoleManager<ApplicationRole> _roleManager;
            public Handler(RoleManager<ApplicationRole> roleManager)
            {
                _roleManager = roleManager;
            }
            public async Task<List<string>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await Task.FromResult(_roleManager.Roles
                    .Select(r => r.Name)
                    .Where(n => n != null)
                    .Select(n => n!)
                    .ToList());
            }
        }
    }
}
