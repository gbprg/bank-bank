using System.ComponentModel.DataAnnotations;

namespace transfer_bank.Models
{
    public class SignInDTO
    {
        [Required(ErrorMessage = "O campo Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]

        public required string Email { get; set; }
        [Required(ErrorMessage = "O campo Password é obrigatório")]
        public required string Password { get; set; }
    }
}