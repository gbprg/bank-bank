using Microsoft.AspNetCore.Mvc;
using transfer_bank.Models;
using transfer_bank.Services;

namespace transfer_bank.Controllers
{
  [ApiController]
  [Route("api/")]
  public class AuthController : ControllerBase
  {
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
      _authService = authService;
    }

    [HttpPost("signin")]
    public async Task<IActionResult> SignIn([FromBody] SignInDTO signInDTO)
    {
      try
      {
        var token = await _authService.SignInAsync(signInDTO);
        return Ok(new { token });
      }
      catch (UnauthorizedAccessException ex)
      {
        return Unauthorized(new { Error = ex.Message });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { Error = "Erro interno no servidor.", Details = ex.Message });
      }
    }
  }
}