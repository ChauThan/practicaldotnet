using App.Application.Abstractions;
using App.Domain;
using MediatR;

namespace App.Application.Feature.Auth;

public static class DisableTwoFactor
{
    public class Command : IRequest<Response>
    {
        public Guid UserId { get; set; }
    }

    public class Response
    {
        public bool Succeeded { get; set; }
        public string? Message { get; set; }
    }

    public class Handler(IUserService userService) : IRequestHandler<Command, Response>
    {
        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await userService.FindByIdAsync(request.UserId);
            if (user == null)
                return new Response { Succeeded = false, Message = "User not found." };

            await userService.SetTwoFactorEnabledAsync(user, false);
            await userService.ResetAuthenticatorKeyAsync(user);
            return new Response { Succeeded = true, Message = "2FA disabled." };
        }
    }
}
