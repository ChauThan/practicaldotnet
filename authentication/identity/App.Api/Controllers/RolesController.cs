using App.Application.Feature.Roles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize(Roles = "Admin")] // Example of role-based authorization
public class RolesController(IMediator mediator) : ControllerBase
{
    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> CreateRole([FromBody] CreateRole.Command command)
    {
        var response = await mediator.Send(command);
        if (response.Succeeded)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    [HttpPost("assign-to-user")]
    [Authorize]
    public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleToUser.Command command)
    {
        var response = await mediator.Send(command);
        if (response.Succeeded)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }
}