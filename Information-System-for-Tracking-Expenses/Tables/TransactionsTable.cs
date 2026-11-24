using System;

namespace InformationSystemForTrackingExpenses.Tables
{
    public class TransactionsTable
    {
        public int id { get; set; } 
        public DateTime transaction_date { get; set; }
        public string type { get; set; } 
        public decimal amount { get; set; }
        public string currency { get; set; }
        public string document_number { get; set; }
        public string description { get; set; }

        
        public int account_id { get; set; }
        public int? counterparty_id { get; set; }
        public int? employee_id { get; set; }
        public int category_id { get; set; }
        public int user_id { get; set; }

        
        public string? account_name { get; set; }
        public string? account_type { get; set; }

        public string? category_name { get; set; }
        public string? category_type { get; set; }

        public string? counterparty_name { get; set; }
        public string? counterparty_type { get; set; }
        public string? counterparty_tax_number { get; set; }

        public string? employee_name { get; set; }
        public string? employee_position { get; set; }

        public string? user_name { get; set; }
        public string? user_email { get; set; }

        
        public TransactionsTable(int id, DateTime transaction_date, string type, decimal amount, 
            string currency, string document_number, string description, 
            string? account_name, string? account_type, 
            string? category_name, string? category_type, 
            string? counterparty_name, string? counterparty_type, string? counterparty_tax_number, 
            string? employee_name, string? employee_position, 
            string? user_name, string? user_email)
        {
            this.id = id;
            this.transaction_date = transaction_date;
            this.type = type;
            this.amount = amount;
            this.currency = currency;
            this.document_number = document_number;
            this.description = description;
            this.account_name = account_name;
            this.account_type = account_type;
            this.category_name = category_name;
            this.category_type = category_type;
            this.counterparty_name = counterparty_name;
            this.counterparty_type = counterparty_type;
            this.counterparty_tax_number = counterparty_tax_number;
            this.employee_name = employee_name;
            this.employee_position = employee_position;
            this.user_name = user_name;
            this.user_email = user_email;
        }

       
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