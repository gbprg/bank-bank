using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using transfer_bank.Data;
using transfer_bank.Helpers;
using transfer_bank.Models;
using BC = BCrypt.Net.BCrypt;

namespace transfer_bank.Services
{
  public class AuthService : IAuthService
  {
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext context, IConfiguration configuration)
    {
      _context = context;
      _configuration = configuration;
    }

    // Cadastro de um novo usuário
    public async Task<User> SignUpAsync(SignUpDTO signUpDTO)
    {
      if (string.IsNullOrWhiteSpace(signUpDTO.Name) ||
          string.IsNullOrWhiteSpace(signUpDTO.Email) ||
          string.IsNullOrWhiteSpace(signUpDTO.Password))
      {
        throw new SignUpException("Todos os campos são obrigatórios.");
      }

      var user = new User
      {
        Id = Guid.NewGuid(),
        Name = signUpDTO.Name,
        Email = signUpDTO.Email,
        Password = BC.HashPassword(signUpDTO.Password),
        CreatedAt = DateTime.UtcNow,
        Balance = 0
      };

      try
      {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
      }
      catch (DbUpdateException)
      {
        throw new SignUpException("Este email já está cadastrado.");
      }
      catch (Exception ex)
      {
        throw new SignUpException($"Erro inesperado ao cadastrar o usuário: {ex.Message}");
      }
    }

    // Login do usuário
    public async Task<string> SignInAsync(SignInDTO signInDTO)
    {
      var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == signInDTO.Email);

      if (user == null || !BC.Verify(signInDTO.Password, user.Password))
      {
        throw new SignInException("Email ou senha inválidos.");
      }

      return GenerateJwtToken(user);
    }

    // Verifica se o usuário tem transações
    public async Task<bool> UserHasTransactionsAsync(Guid userId)
    {
      return await _context.Transactions
        .AnyAsync(t => t.SenderId == userId || t.ReceiverId == userId);
    }

    // Exclui um usuário pelo seu ID, se não tiver transações
    public async Task DeleteUserAsync(Guid userId)
    {
      if (await UserHasTransactionsAsync(userId))
      {
        throw new DeleteUserException("Este usuário possui transações e não pode ser excluído.");
      }

      var user = await _context.Users.FindAsync(userId);
      if (user == null)
      {
        throw new DeleteUserException("Usuário não encontrado.");
      }

      try
      {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        throw new DeleteUserException($"Erro inesperado ao excluir o usuário: {ex.Message}");
      }
    }

    // Geração do token JWT
    private string GenerateJwtToken(User user)
    {
      try
      {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key não configurado")));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
          new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
          new Claim(JwtRegisteredClaimNames.Email, user.Email),
          new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
          issuer: _configuration["Jwt:Issuer"],
          audience: _configuration["Jwt:Audience"],
          claims: claims,
          expires: DateTime.Now.AddHours(5),
          signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException($"Erro ao gerar o token JWT: {ex.Message}");
      }
    }

  }
}