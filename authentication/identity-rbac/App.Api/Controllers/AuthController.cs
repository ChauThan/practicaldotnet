﻿using App.Application.Feature.Auth;
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
        if (response.Succeeded)
        {
            return Ok(response);
        }
        return Unauthorized(response);
    }

    // List all users
    [HttpGet("users")]
    public async Task<IActionResult> ListUsers()
    {
        var users = await mediator.Send(new ListUsers.Query());
        return Ok(users);
    }
}