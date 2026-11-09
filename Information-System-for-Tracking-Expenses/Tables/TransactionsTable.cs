using System;

namespace FinancialApp.Tables
{
    public class TransactionsTable
    {
        public int id { get; set; }
        public int account_id { get; set; }
        public int? counterparty_id { get; set; }
        public int? employee_id { get; set; }
        public int category_id { get; set; }
        public int user_id { get; set; }
        public string type { get; set; } 
        public decimal amount { get; set; }
        public string currency { get; set; }
        public DateTime transaction_date { get; set; }
        public string description { get; set; }
        public string document_number { get; set; }

        public TransactionsTable(int id, int account_id, int? counterparty_id, int? employee_id, 
            int category_id, int user_id, string type, decimal amount, 
            string currency, DateTime transaction_date, string description, string document_number)
        {
            this.id = id;
            this.account_id = account_id;
            this.counterparty_id = counterparty_id;
            this.employee_id = employee_id;
            this.category_id = category_id;
            this.user_id = user_id;
            this.type = type;
            this.amount = amount;
            this.currency = currency;
            this.transaction_date = transaction_date;
            this.description = description;
            this.document_number = document_number;
        }
    }
}