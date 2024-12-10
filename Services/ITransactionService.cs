using System.Security.Claims;
using transfer_bank.Models;

namespace transfer_bank.Services
{
    public interface ITransactionService
    {
        Task<TransactionResponse> DepositAsync(DepositDTO depositDTO);
        Task<TransactionResponse> TransferAsync(TransferDTO transferDto, Guid senderId);
        Task<TransactionResponse> ReverseTransactionAsync(Guid userId, int page = 1, int pageSize = 10);
        Task<List<TransactionResponse>> GetUserTransactionsAsync(Guid userId, int page = 1, int pageSize = 10);
        Task<string?> ExportAndSaveTransactionsAsync(Guid userId, string filter = "all", string? monthYear = null);
    }
}