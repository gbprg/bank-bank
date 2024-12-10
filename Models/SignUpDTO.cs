using System.ComponentModel.DataAnnotations;

namespace transfer_bank.Models
{
    // Representa os dados necessários para cadastrar um usuário
    public class SignUpDTO
    {
        [Required(ErrorMessage = "Email é necessário")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é necessária")]
        [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres")]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[\W_]).{6,}$", ErrorMessage = "A senha deve conter letras, números e caracteres especiais.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nome é necessário")]
        public string Name { get; set; } = string.Empty;
    }
}