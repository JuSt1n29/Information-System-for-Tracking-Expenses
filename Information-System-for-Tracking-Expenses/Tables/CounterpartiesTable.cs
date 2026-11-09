namespace FinancialApp.Tables
{
    public class CounterpartiesTable
    {
        public int id { get; set; }
        public string name { get; set; }
        public string type { get; set; } 
        public string tax_number { get; set; }
        public string phone { get; set; }
        public string email { get; set; }

        public CounterpartiesTable(int id, string name, string type, string tax_number, string phone, string email)
        {
            this.id = id;
            this.name = name;
            this.type = type;
            this.tax_number = tax_number;
            this.phone = phone;
            this.email = email;
        }
    }
}