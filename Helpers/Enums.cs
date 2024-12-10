namespace transfer_bank.helpers
{
    public static class Enums
    {
        public enum TransactionType
        {
            Deposit,
            Transfer,
            Reversal
        }

        public enum PaymentMethod
        {
            Cash,
            CreditCard,
            DebitCard,
            Pix,
            Reversal
        }
    }
}