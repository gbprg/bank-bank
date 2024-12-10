using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace transfer_bank.Models
{
  // Representa um usuário da aplicação
  public class User
  {
    public Guid Id { get; set; }

    [EmailAddress(ErrorMessage = "Formato de email inválido")]
    public string Email { get; set; } = string.Empty;

    [JsonIgnore]
    public string Password { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Range(0, double.MaxValue, ErrorMessage = "O saldo não pode ser negativo.")]
    public decimal Balance { get; internal set; }
  }
}
