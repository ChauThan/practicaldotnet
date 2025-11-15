using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ShortLink.Infrastructure.Identity;
using ShortLink.Api.Services;

namespace ShortLink.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;

    public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (request == null)
            return BadRequest();

        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null)
            return Unauthorized();

        var valid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!valid)
            return Unauthorized();

        var token = await _tokenService.CreateTokenAsync(user);
        return Ok(new { token = token });
    }
}
