using App.Application.Abstractions;
using App.Domain;
using MediatR;

namespace App.Application.Feature.Auth;

public static class VerifyTwoFactorSetup
{
    public class Command : IRequest<Response>
    {
        public Guid UserId { get; set; }
        public string Code { get; set; } = string.Empty;
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

            var is2FAEnabled = await userService.GetTwoFactorEnabledAsync(user);
            if (is2FAEnabled)
                return new Response { Succeeded = false, Message = "2FA is already enabled for this user." };

            var isValidCode = await userService.VerifyTwoFactorTokenAsync(user, request.Code);
            if (isValidCode)
            {
                await userService.SetTwoFactorEnabledAsync(user, true);
                return new Response { Succeeded = true, Message = "2FA enabled successfully." };
            }
            return new Response { Succeeded = false, Message = "Invalid code. Please check your authenticator app and try again." };
        }
    }
}
