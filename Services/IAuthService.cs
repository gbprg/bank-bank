using transfer_bank.Models;

namespace transfer_bank.Services
{
    public interface IAuthService
    {
        Task<User> SignUpAsync(SignUpDTO signUpDTO);
        Task<string> SignInAsync(SignInDTO signInDTO);
        Task DeleteUserAsync(Guid userId);
        Task<bool> UserHasTransactionsAsync(Guid userId);
    }
}