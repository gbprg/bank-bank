using System.Security.Claims;
using System.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using transfer_bank.Helpers;
using transfer_bank.Models;
using transfer_bank.Services;

namespace transfer_bank.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(ITransactionService transactionService, ILogger<TransactionsController> logger)
        {
            _transactionService = transactionService;
            _logger = logger;
        }

        [HttpPost("deposit")]
        [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Deposit([FromBody] DepositDTO depositDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var transaction = await _transactionService.DepositAsync(depositDto);
                return Ok(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao realizar o depósito.");
                return StatusCode(500, new { Error = "Erro interno no servidor." });
            }
        }

        [HttpPost("transfer")]
        [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Transfer([FromBody] TransferDTO transferDto)
        {
            try
            {
                var senderIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(senderIdClaim))
                {
                    return BadRequest(new { Error = "Usuário não autenticado." });
                }

                var senderId = Guid.Parse(senderIdClaim);

                var transaction = await _transactionService.TransferAsync(transferDto, senderId);
                return Ok(transaction);
            }
            catch (TransferException ex)
            {
                _logger.LogError(ex, "Erro ao realizar a transferência.");
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao realizar a transferência.");
                return StatusCode(500, new { Error = "Erro interno no servidor." });
            }
        }

        [HttpGet("history")]
        [ProducesResponseType(typeof(List<TransactionResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTransactions([FromQuery] Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest(new
                {
                    ErrorCode = "BadRequest",
                    Message = "O ID do usuário não pode ser vazio."
                });
            }

            try
            {
                var transactions = await _transactionService.GetUserTransactionsAsync(userId, page, pageSize);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter transações do usuário.");
                return StatusCode(500, new { Error = "Erro interno no servidor." });
            }
        }

        [HttpPost("reverse")]
        [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ReverseTransaction([FromQuery] Guid transactionId)
        {
            try
            {
                var reversedTransaction = await _transactionService.ReverseTransactionAsync(transactionId);
                return Ok(reversedTransaction);
            }
            catch (BankTransactionException ex)
            {
                _logger.LogError(ex, "Erro ao reverter transação.");
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao reverter transação.");
                return StatusCode(500, new { Error = "Erro interno no servidor." });
            }
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportAndSaveTransactions([FromQuery] Guid userId, [FromQuery] string filter = "all", [FromQuery] string? monthYear = null)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest(new
                {
                    ErrorCode = "BadRequest",
                    Message = "O ID do usuário não pode ser vazio."
                });
            }

            try
            {
                var filePath = await _transactionService.ExportAndSaveTransactionsAsync(userId, filter, monthYear);

                if (filePath == null)
                {
                    return NotFound(new { Message = "Você não tem movimentos neste período." });
                }

                return Ok(new
                {
                    Message = "Arquivo CSV gerado com sucesso.",
                    FilePath = filePath
                });
            }
            catch (BankTransactionException ex)
            {
                _logger.LogWarning(ex, "Erro de validação ao exportar transações.");
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao exportar transações.");
                return StatusCode(500, new { Error = "Erro interno no servidor." });
            }
        }
    }
}