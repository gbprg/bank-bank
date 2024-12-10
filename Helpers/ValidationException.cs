namespace transfer_bank.Helpers
{
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }
    }

    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message) { }
    }

    public class DepositException : Exception
    {
        public DepositException(string message) : base(message) { }
    }

    public class TransferException : Exception
    {
        public TransferException(string message) : base(message) { }
    }

    public class BankTransactionException : Exception
    {
        public BankTransactionException(string message) : base(message) { }
    }

    public class SignUpException : Exception
    {
        public SignUpException(string message) : base(message) { }
    }

    public class SignInException : Exception
    {
        public SignInException(string message) : base(message) { }
    }

    public class DeleteUserException : Exception
    {
        public DeleteUserException(string message) : base(message) { }
    }
}