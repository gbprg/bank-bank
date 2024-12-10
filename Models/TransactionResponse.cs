using static transfer_bank.helpers.Enums;

namespace transfer_bank.Models
{
    // Representa uma resposta de transação, o que vai ser retornado ao usuário
    public class TransactionResponse
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsReversed { get; set; }

        public string? SenderName { get; set; }
        public string? SenderEmail { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverEmail { get; set; } = string.Empty;

        public string FormattedAmount => $"R$ {Amount:N2}";
        public string Status => IsReversed ? "Revertida" : "Concluída";
    }

}