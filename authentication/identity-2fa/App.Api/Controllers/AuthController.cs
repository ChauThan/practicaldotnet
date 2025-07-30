using App.Application.Feature.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator)
    : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] Register.Command command)
    {
        var response = await mediator.Send(command);
        if (response.Succeeded)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Login.Query query)
    {
        var response = await mediator.Send(query);
        if (response.RequiresTwoFactor)
        {
            return Ok(new
            {
                requiresTwoFactor = true,
                userId = response.UserId,
                message = "Two-factor authentication required."
            });
        }
        if (response.Succeeded)
        {
            return Ok(response);
        }
        return Unauthorized(response);
    }

    [HttpPost("login-2fa")]
    public async Task<IActionResult> LoginWith2FA([FromBody] LoginWith2FA.Command command)
    {
        var response = await mediator.Send(command);
        if (response.Succeeded)
        {
            return Ok(response);
        }
        return Unauthorized(response);
    }
    [HttpPost("2fa/enable")]
    public async Task<IActionResult> EnableTwoFactor([FromBody] EnableTwoFactor.Command command)
    {
        var response = await mediator.Send(command);
        return Ok(response);
    }

    [HttpPost("2fa/verify")]
    public async Task<IActionResult> VerifyTwoFactor([FromBody] VerifyTwoFactorSetup.Command command)
    {
        var response = await mediator.Send(command);
        if (response.Succeeded)
            return Ok(response);
        return BadRequest(response);
    }

    [HttpPost("2fa/disable")]
    public async Task<IActionResult> DisableTwoFactor([FromBody] DisableTwoFactor.Command command)
    {
        var response = await mediator.Send(command);
        if (response.Succeeded)
            return Ok(response);
        return BadRequest(response);
    }

    [HttpPost("2fa/recovery-codes")]
    public async Task<IActionResult> GenerateRecoveryCodes([FromBody] GenerateRecoveryCodes.Command command)
    {
        var response = await mediator.Send(command);
        return Ok(response);
    }
}