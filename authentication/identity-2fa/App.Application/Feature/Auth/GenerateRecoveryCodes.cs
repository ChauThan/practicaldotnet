using App.Application.Abstractions;
using App.Domain;
using MediatR;

namespace App.Application.Feature.Auth;

public static class GenerateRecoveryCodes
{
    public class Command : IRequest<Response>
    {
        public Guid UserId { get; set; }
        public int Number { get; set; } = 10;
    }

    public class Response
    {
        public IEnumerable<string> RecoveryCodes { get; set; } = [];
    }

    public class Handler(IUserService userService) : IRequestHandler<Command, Response>
    {
        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await userService.FindByIdAsync(request.UserId);
            if (user == null)
                return new Response();

            var codes = await userService.GenerateNewTwoFactorRecoveryCodesAsync(user, request.Number);
            return new Response { RecoveryCodes = codes };
        }
    }
}
