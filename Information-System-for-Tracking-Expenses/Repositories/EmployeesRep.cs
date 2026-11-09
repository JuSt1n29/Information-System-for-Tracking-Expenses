using System;
using System.Collections.Generic;
using System.Globalization;
using FinancialApp.Tables;

namespace FinancialApp.Repositories
{
    internal class EmployeesRep
    {
        DB db = DB.getInstance();

        public EmployeesTable GetbyID(int id)
        {
            foreach (EmployeesTable k in GetAll())
                if (k.id == id) return k;
            return null;
        }

        public IEnumerable<EmployeesTable> GetAll()
        {
            List<EmployeesTable> list = new List<EmployeesTable>();
            try
            {
                using (var reader = db.execSelect("SELECT id, full_name, position, phone, email, salary, hire_date, status FROM employees"))
                {
                    if (reader != null)
                        while (reader.Read())
                            list.Add(new EmployeesTable(
                                reader.GetInt32("id"),
                                reader.GetString("full_name"),
                                reader.IsDBNull(reader.GetOrdinal("position")) ? null : reader.GetString("position"),
                                reader.IsDBNull(reader.GetOrdinal("phone")) ? null : reader.GetString("phone"),
                                reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString("email"),
                                reader.IsDBNull(reader.GetOrdinal("salary")) ? 0 : reader.GetDecimal("salary"),
                                reader.IsDBNull(reader.GetOrdinal("hire_date")) ? DateTime.MinValue : reader.GetDateTime("hire_date"),
                                reader.GetString("status")
                            ));
                }
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
            return list;
        }

        public bool Add(EmployeesTable entity)
        {
            try 
            { 
                string pos = entity.position != null ? $"'{entity.position}'" : "NULL";
                string phone = entity.phone != null ? $"'{entity.phone}'" : "NULL";
                string email = entity.email != null ? $"'{entity.email}'" : "NULL";
                string salary = entity.salary.ToString(CultureInfo.InvariantCulture);
                string hireDate = entity.hire_date != DateTime.MinValue ? $"'{entity.hire_date:yyyy-MM-dd}'" : "NULL";

                return db.execSql($"INSERT INTO employees (id, full_name, position, phone, email, salary, hire_date, status) VALUES ({entity.id}, '{entity.full_name}', {pos}, {phone}, {email}, {salary}, {hireDate}, '{entity.status}')"); 
            }
            catch (Exception e) { Console.WriteLine(e.Message); return false; }
        }

        public bool Delete(EmployeesTable entity)
        {
            try 
            { 
                return db.execSql($"DELETE FROM employees WHERE id = {entity.id}"); 
            }
            catch (Exception e) { Console.WriteLine(e.Message); return false; }
        }

        public bool Update(EmployeesTable entity)
        {
            try 
            { 
                string pos = entity.position != null ? $"'{entity.position}'" : "NULL";
                string phone = entity.phone != null ? $"'{entity.phone}'" : "NULL";
                string email = entity.email != null ? $"'{entity.email}'" : "NULL";
                string salary = entity.salary.ToString(CultureInfo.InvariantCulture);
                string hireDate = entity.hire_date != DateTime.MinValue ? $"'{entity.hire_date:yyyy-MM-dd}'" : "NULL";

                return db.execSql($"UPDATE employees SET full_name = '{entity.full_name}', position = {pos}, phone = {phone}, email = {email}, salary = {salary}, hire_date = {hireDate}, status = '{entity.status}' WHERE id = {entity.id}"); 
            }
            catch (Exception e) { Console.WriteLine(e.Message); return false; }
        }
    }
}