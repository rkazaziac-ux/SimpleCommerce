namespace SimpleCommerce.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ApiControllerBase
{
    /// <summary>Registers a new user with the Customer (default) or Admin role.</summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.RegisterAsync(request, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    /// <summary>Logs in and returns a signed JWT to use in the Authorize button of Swagger.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(request, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : Unauthorized(new { error = result.Error });
    }
}
