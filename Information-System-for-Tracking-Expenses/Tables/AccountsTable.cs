namespace InformationSystemForTrackingExpenses.Tables
{
    public class AccountsTable
    {
        public int id { get; set; }
        public string name { get; set; }
        public string type { get; set; } // 'bank' / 'cash'
        public string currency { get; set; }
        public decimal balance { get; set; }

        public AccountsTable(int id, string name, string type, string currency, decimal balance)
        {
            this.id = id;
            this.name = name;
            this.type = type;
            this.currency = currency;
            this.balance = balance;
        }
    }
}