using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using transfer_bank.Data;
using transfer_bank.Helpers;
using transfer_bank.Models;
using transfer_bank.Services;

namespace transfer_bank.Controllers
{
  [ApiController]
  [Route("api/")]
  public class UserController : ControllerBase
  {
    private readonly IAuthService _authService;
    private readonly ILogger<UserController> _logger;
    private readonly AppDbContext _context;

    public UserController(IAuthService authService, ILogger<UserController> logger, AppDbContext context)
    {
      _authService = authService;
      _logger = logger;
      _context = context;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] SignUpDTO signUpDTO)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      try
      {
        var user = await _authService.SignUpAsync(signUpDTO);
        return CreatedAtAction(nameof(SignUp), new { id = user.Id }, user);
      }
      catch (SignUpException ex)
      {
        return BadRequest(new { Error = ex.Message });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erro inesperado ao salvar o usuário.");
        return StatusCode(500, new { Error = "Erro interno no servidor." });
      }
    }

    [HttpDelete("user/{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
      try
      {
        await _authService.DeleteUserAsync(id);
        return Ok(new { Message = "Usuário excluído com sucesso" });
      }
      catch (DeleteUserException ex)
      {
        return BadRequest(new { Error = ex.Message });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erro inesperado ao excluir o usuário.");
        return StatusCode(500, new { Error = "Erro interno no servidor." });
      }
    }

  }
}