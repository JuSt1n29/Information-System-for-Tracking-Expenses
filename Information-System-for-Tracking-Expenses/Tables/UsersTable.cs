using System;

namespace FinancialApp.Tables
{
    public class UsersTable
    {
        public int id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string password_hash { get; set; }
        public string role { get; set; }
        public DateTime created_at { get; set; }

        public UsersTable(int id, string name, string email, string password_hash, string role, DateTime created_at)
        {
            this.id = id;
            this.name = name;
            this.email = email;
            this.password_hash = password_hash;
            this.role = role;
            this.created_at = created_at;
        }
    }
}