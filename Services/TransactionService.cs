using System.Globalization;
using System.Security.Claims;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using transfer_bank.Data;
using transfer_bank.Helpers;
using transfer_bank.Models;
using static transfer_bank.helpers.Enums;

namespace transfer_bank.Services
{

    public class TransactionService : ITransactionService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TransactionService> _logger;

        public TransactionService(AppDbContext context, ILogger<TransactionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Processa um depósito na conta de um usuário.
        public async Task<TransactionResponse> DepositAsync(DepositDTO depositDto)
        {
            try
            {
                if (depositDto == null)
                    throw new DepositException("Informações de depósito não podem ser nulas");

                // Obtém o usuário associado ao depósito.
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == depositDto.UserId);

                if (user == null)
                    throw new DepositException($"Usuário com ID {depositDto.UserId} não encontrado");

                if (depositDto.Amount <= 0)
                    throw new DepositException("O valor do depósito deve ser maior que zero");

                user.Balance += depositDto.Amount;

                // Cria a transação de depósito.
                var transaction = new Transaction()
                {
                    SenderId = depositDto.UserId,
                    ReceiverId = depositDto.UserId,
                    Amount = depositDto.Amount,
                    Type = TransactionType.Deposit,
                    PaymentMethod = depositDto.PaymentMethod,
                    Receiver = user
                };

                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();

                // Cria a resposta com os detalhes da transação.
                var transactionResponse = new TransactionResponse()
                {
                    Id = transaction.Id,
                    ReceiverEmail = user.Email,
                    ReceiverName = user.Name,
                    ReceiverId = transaction.ReceiverId,
                    Amount = transaction.Amount,
                    Type = transaction.Type,
                    CreatedAt = transaction.CreatedAt,
                    IsReversed = transaction.IsReversed,
                };

                _logger.LogInformation($"Depósito de {depositDto.Amount} realizado pelo usuário {depositDto.UserId}");

                return transactionResponse;
            }
            catch (DepositException ex)
            {
                _logger.LogError(ex, $"Erro ao processar depósito para usuário {depositDto?.UserId}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao processar depósito.");
                throw new BankTransactionException("Ocorreu um erro inesperado ao processar o depósito.");
            }
        }

        // Processa uma transferência de valores entre usuários.
        public async Task<TransactionResponse> TransferAsync(TransferDTO transferDto, Guid senderId)
        {
            try
            {
                // Obtém o remetente da transferência.
                var sender = await _context.Users
                    .Where(u => u.Id == senderId)
                    .FirstOrDefaultAsync();

                if (sender == null)
                {
                    throw new TransferException("Remetente não encontrado.");
                }

                // Obtém o receptor da transferência.
                var receiver = await _context.Users
                    .Where(u => u.Id == transferDto.ReceiverId)
                    .FirstOrDefaultAsync();

                if (receiver == null)
                {
                    throw new TransferException("Destinatário não encontrado.");
                }

                if (senderId == transferDto.ReceiverId)
                    throw new TransferException("Você não pode transferir dinheiro para si mesmo.");

                if (transferDto.Amount <= 0)
                    throw new TransferException("O valor da transferência deve ser maior que zero");

                if (sender.Balance < transferDto.Amount)
                    throw new TransferException("Saldo insuficiente");

                sender.Balance -= transferDto.Amount;
                receiver.Balance += transferDto.Amount;

                // Cria a transação de transferência.
                var transaction = new Transaction
                {
                    Id = Guid.NewGuid(),
                    SenderId = sender.Id,
                    ReceiverId = transferDto.ReceiverId,
                    Amount = transferDto.Amount,
                    Type = TransactionType.Transfer,
                    PaymentMethod = transferDto.PaymentMethod,
                    Sender = sender,
                    Receiver = receiver
                };

                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();

                // Cria a resposta com os detalhes da transação.
                var transactionResponse = new TransactionResponse()
                {
                    Id = transaction.Id,
                    SenderId = transaction.SenderId,
                    ReceiverId = transaction.ReceiverId,
                    Amount = transaction.Amount,
                    Type = transaction.Type,
                    PaymentMethod = transaction.PaymentMethod,
                    CreatedAt = transaction.CreatedAt,
                    IsReversed = transaction.IsReversed,
                    SenderName = sender.Name,
                    SenderEmail = sender.Email,
                    ReceiverName = receiver.Name,
                    ReceiverEmail = receiver.Email
                };

                _logger.LogInformation($"Transferência de {transferDto.Amount} de {sender.Id} para {transferDto.ReceiverId}");

                return transactionResponse;
            }
            catch (TransferException ex)
            {
                _logger.LogError(ex, $"Erro ao processar transferência de {transferDto.ReceiverId} para {transferDto.ReceiverId}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao processar transferência.");
                throw new BankTransactionException("Ocorreu um erro inesperado ao processar a transferência.");
            }
        }

        // Reverte uma transação já realizada.
        public async Task<TransactionResponse> ReverseTransactionAsync(Guid transactionId, int page = 1, int pageSize = 10)
        {
            // Obtém a transação original.
            var originalTransaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == transactionId);

            if (originalTransaction == null || originalTransaction.IsReversed)
                throw new ArgumentException("Transação não encontrada ou já revertida");

            // Cria um DTO para a reversão da transação.
            var transferDto = new TransferDTO(originalTransaction.SenderId, originalTransaction.Amount, PaymentMethod.Reversal);

            // Chama o método para transferir (reverter a transação).
            var reversalTransaction = await TransferAsync(transferDto, originalTransaction.ReceiverId);

            originalTransaction.IsReversed = true;
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Transação revertida {transactionId}");

            return reversalTransaction;
        }

        // Retonar uma lista de todas transações de um usuário com paginação.
        public async Task<List<TransactionResponse>> GetUserTransactionsAsync(Guid userId, int page = 1, int pageSize = 10)
        {
            // Obtém as transações relacionadas ao usuário, filtrando por remetente ou receptor.
            var transactions = await _context.Transactions
                .AsNoTracking()
                .Where(t => t.SenderId == userId || t.ReceiverId == userId)
                .Select(t => new
                {
                    // Para cada transação, obtém o remetente e o receptor do banco de dados.
                    Transaction = t,
                    Sender = _context.Users.AsNoTracking().FirstOrDefault(u => u.Id == t.SenderId),
                    Receiver = _context.Users.AsNoTracking().FirstOrDefault(u => u.Id == t.ReceiverId)
                })
                .Select(t => new TransactionResponse
                {
                    // Preenche a resposta com os detalhes da transação.
                    Id = t.Transaction.Id,
                    SenderId = t.Transaction.SenderId,
                    ReceiverId = t.Transaction.ReceiverId,
                    Amount = t.Transaction.Amount,
                    Type = t.Transaction.Type,
                    PaymentMethod = t.Transaction.PaymentMethod,
                    CreatedAt = t.Transaction.CreatedAt,
                    IsReversed = t.Transaction.IsReversed,
                    SenderName = t.Sender != null ? t.Sender.Name : "Desconhecido",
                    SenderEmail = t.Sender != null ? t.Sender.Email : "Desconhecido",
                    ReceiverName = t.Receiver != null ? t.Receiver.Name : "Desconhecido",
                    ReceiverEmail = t.Receiver != null ? t.Receiver.Email : "Desconhecido"
                })
                .OrderBy(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return transactions;
        }

        // Exporta um arquivo CSV com todas as transações de um usuário, com a opção de filtrar por data ou por um período específico.
        public async Task<string?> ExportAndSaveTransactionsAsync(Guid userId, string filter = "all", string? monthYear = null)
        {
            try
            {
                // Inicia a busca pelas transações do usuário, tanto como remetente quanto como receptor.
                var query = _context.Transactions
                    .AsNoTracking()
                    .Where(t => t.SenderId == userId || t.ReceiverId == userId);

                if (filter.ToLower() == "month")
                {
                    query = ApplyMonthFilter(query, monthYear);
                }
                else if (filter.ToLower() == "last30days")
                {
                    query = query.Where(t => t.CreatedAt >= DateTime.UtcNow.AddDays(-30));
                }

                var transactions = await query
                    .OrderByDescending(t => t.CreatedAt)
                    .Select(t => new TransactionResponse
                    {
                        // Faz o mapeamento dos dados para o formato esperado pela resposta.
                        Id = t.Id,
                        SenderId = t.SenderId,
                        ReceiverId = t.ReceiverId,
                        Amount = t.Amount,
                        Type = t.Type,
                        PaymentMethod = t.PaymentMethod,
                        CreatedAt = t.CreatedAt,
                        IsReversed = t.IsReversed,
                        SenderName = t.Sender != null ? t.Sender.Name : "Desconhecido",
                        SenderEmail = t.Sender != null ? t.Sender.Email : "Desconhecido",
                        ReceiverName = t.Receiver != null ? t.Receiver.Name : "Desconhecido",
                        ReceiverEmail = t.Receiver != null ? t.Receiver.Email : "Desconhecido"
                    })
                    .ToListAsync();

                // Verifica se a lista de transações está vazia.
                // Se não houver transações, retorna null.
                if (!transactions.Any())
                {
                    return null;
                }

                var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "ExportedFiles");
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                var filePath = Path.Combine(directoryPath, $"transactions_{DateTime.UtcNow:yyyyMMddHHmmss}.csv");

                var csvContent = new StringBuilder();
                csvContent.AppendLine("Id,Tipo de pagamento,Metodo de pagamento,Criado em,É Revertido,Nome do remetente,Email do remetente,Nome do receptor,Email do receptor,Valor");

                foreach (var transaction in transactions)
                {
                    var formattedAmount = $"R$ {transaction.Amount:N2}";

                    var formattedLine = $"\"{transaction.Id}\"," +
                                        $"\"{transaction.Type}\"," +
                                        $"\"{transaction.PaymentMethod}\"," +
                                        $"\"{transaction.CreatedAt:yyyy-MM-dd HH:mm:ss}\"," +
                                        $"\"{(transaction.IsReversed ? "Sim" : "Não")}\"," +
                                        $"\"{transaction.SenderName}\"," +
                                        $"\"{transaction.SenderEmail}\"," +
                                        $"\"{transaction.ReceiverName}\"," +
                                        $"\"{transaction.ReceiverEmail}\"," +
                                        $"\"{formattedAmount}\"";

                    csvContent.AppendLine(formattedLine);
                }

                await File.WriteAllTextAsync(filePath, csvContent.ToString());

                return filePath;
            }
            catch (BankTransactionException ex)
            {
                _logger.LogError(ex, "Erro ao exportar transações.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao exportar transações.");
                throw new BankTransactionException("Ocorreu um erro inesperado ao exportar as transações.");
            }
        }

        // Aplica o filtro de data (mês e ano) na consulta de transações.
        private IQueryable<Transaction> ApplyMonthFilter(IQueryable<Transaction> query, string? monthYear)
        {
            if (string.IsNullOrEmpty(monthYear))
                throw new ArgumentException("MonthYear deve ser fornecido para o filtro 'month'.");

            if (!DateTime.TryParseExact(monthYear, "MM/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
            {
                throw new ArgumentException("O formato de MonthYear deve ser 'MM/yy'.");
            }

            if (parsedDate > DateTime.UtcNow)
            {
                throw new ArgumentException("A data fornecida não pode ser no futuro.");
            }

            // Filtra as transações, selecionando apenas aquelas que correspondem ao mês e ano fornecidos.
            var filteredQuery = query.Where(t =>
                t.CreatedAt.Month == parsedDate.Month &&
                t.CreatedAt.Year == parsedDate.Year
            );

            return filteredQuery;
        }
    }
}