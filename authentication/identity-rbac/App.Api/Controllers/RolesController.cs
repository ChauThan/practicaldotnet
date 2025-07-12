using App.Application.Feature.Roles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
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

    // List all roles
    [HttpGet("all")]
    public async Task<IActionResult> ListRoles()
    {
        var roles = await mediator.Send(new ListRoles.Query());
        return Ok(roles);
    }

    // Remove a role from a user
    [HttpPost("remove-from-user")]
    public async Task<IActionResult> RemoveRoleFromUser([FromBody] RemoveRoleFromUser.Command command)
    {
        var response = await mediator.Send(command);
        if (response.Succeeded)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    // List roles assigned to a user
    [HttpGet("user-roles/{userId}")]
    public async Task<IActionResult> ListUserRoles(string userId)
    {
        var roles = await mediator.Send(new ListUserRoles.Query(userId));
        return Ok(roles);
    }
}