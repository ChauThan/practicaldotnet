using MediatR;
using Microsoft.AspNetCore.Identity;
using App.Domain;

namespace App.Application.Feature.Auth
{
    public class ListUsers
    {
        public record Query : IRequest<List<UserDto>>;
        public record UserDto(string Id, string UserName, string Email);

        public class Handler : IRequestHandler<Query, List<UserDto>>
        {
            private readonly UserManager<ApplicationUser> _userManager;
            public Handler(UserManager<ApplicationUser> userManager)
            {
                _userManager = userManager;
            }
            public async Task<List<UserDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var users = _userManager.Users.ToList();
                return await Task.FromResult(users.Select(u => new UserDto(u.Id.ToString(), u.UserName ?? string.Empty, u.Email ?? string.Empty)).ToList());
            }
        }
    }
}
