using App.Application.Abstractions;
using App.Domain;
using MediatR;

namespace App.Application.Feature.Auth;

public static class EnableTwoFactor
{
    public class Command : IRequest<Response>
    {
        public Guid UserId { get; set; }
    }

    public class Response
    {
        public string? SharedKey { get; set; }
        public string? AuthenticatorUri { get; set; }
    }

    public class Handler(IUserService userService) : IRequestHandler<Command, Response>
    {
        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await userService.FindByIdAsync(request.UserId);
            if (user == null)
                return new Response();

            var key = await userService.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(key))
            {
                await userService.ResetAuthenticatorKeyAsync(user);
                key = await userService.GetAuthenticatorKeyAsync(user);
            }

            // Generate URI for authenticator apps
            var authenticatorUri = $"otpauth://totp/App:{user.Email}?secret={key}&issuer=App";

            return new Response
            {
                SharedKey = key,
                AuthenticatorUri = authenticatorUri
            };
        }
    }
}
