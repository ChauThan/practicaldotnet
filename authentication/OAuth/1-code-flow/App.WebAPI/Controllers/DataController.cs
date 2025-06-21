using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace App.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")] // Route của API
public class DataController : ControllerBase
{
    [HttpGet("read")]
    [Authorize(Policy = "ApiReadScope")] // Yêu cầu scope "api.read"
    public IActionResult GetReadData()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userName = User.FindFirst("name")?.Value;
        var email = User.FindFirst("email")?.Value;
        var preferredUsername = User.FindFirst("preferred_username")?.Value;

        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();

        return Ok(new
        {
            Message = $"Hello {userName ?? userId}! You have successfully accessed the protected API (Read Data).",
            UserId = userId,
            Email = email,
            PreferredUsername = preferredUsername,
            Claims = claims,
            ApiAccessed = true
        });
    }

    [HttpPost("write")]
    [Authorize(Policy = "ApiWriteScope")] // Yêu cầu scope "api.write"
    public IActionResult PostWriteData()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userName = User.FindFirst("name")?.Value;

        return Ok(new
        {
            Message = $"Hello {userName ?? userId}! Data has been successfully written (Write Data).",
            ApiAccessed = true
        });
    }
}