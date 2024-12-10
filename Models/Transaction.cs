using System.ComponentModel.DataAnnotations;
using static transfer_bank.helpers.Enums;

namespace transfer_bank.Models
{
    // Representa uma transação financeira
    public class Transaction
    {
        [Required(ErrorMessage = "O ID do usuário é obrigatório.")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O ID do remetente é obrigatório.")]
        public Guid SenderId { get; set; }

        [Required(ErrorMessage = "O ID do destinatário é obrigatório.")]
        public Guid ReceiverId { get; set; }

        [Required(ErrorMessage = "O valor é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
        public decimal Amount { get; set; }

        public TransactionType Type { get; set; }

        [Required(ErrorMessage = "O meio de pagamento é obrigatório.")]
        public PaymentMethod PaymentMethod { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsReversed { get; set; }

        public User? Sender { get; set; }
        public User? Receiver { get; set; }

        public string FormattedAmount => $"R$ {Amount:N2}";
    }

    public record DepositDTO(Guid UserId, decimal Amount, PaymentMethod PaymentMethod);
    public record TransferDTO(Guid ReceiverId, decimal Amount, PaymentMethod PaymentMethod);
    public record TransactionFilterDTO(Guid UserId, string Filter = "all", string? MonthYear = null);
}
