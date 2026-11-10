using System;

namespace InformationSystemForTrackingExpenses.Tables
{
    public class EmployeesTable
    {
        public int id { get; set; }
        public string full_name { get; set; }
        public string position { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public decimal salary { get; set; }
        public DateTime hire_date { get; set; }
        public string status { get; set; } // 'active', 'inactive', 'terminated'

        public EmployeesTable(int id, string full_name, string position, string phone, string email, decimal salary, DateTime hire_date, string status)
        {
            this.id = id;
            this.full_name = full_name;
            this.position = position;
            this.phone = phone;
            this.email = email;
            this.salary = salary;
            this.hire_date = hire_date;
            this.status = status;
        }
    }
}