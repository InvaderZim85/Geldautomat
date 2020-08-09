namespace Geldautomat.DataObjects
{
    /// <summary>
    /// Represents a bank account
    /// </summary>
    internal sealed class BankAccount
    {
        /// <summary>
        /// Gets or sets the id of the bank account
        /// </summary>
        public uint Id { get; set; }

        /// <summary>
        /// Gets or sets the bank account number
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Gets or sets the pin of the bank account
        /// </summary>
        public uint Pin { get; set; }

        /// <summary>
        /// Gets or sets the name of the person who owns the bank account
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the bank amount
        /// </summary>
        public decimal Amount { get; set; }
    }
}
