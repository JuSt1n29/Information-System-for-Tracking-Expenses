using System;
using System.Collections.Generic;
using InformationSystemForTrackingExpenses.Tables;

namespace  InformationSystemForTrackingExpenses.Repositories
{
    internal class CounterpartiesRep
    {
        DB db = DB.getInstance();

        public CounterpartiesTable GetbyID(int id)
        {
            foreach (CounterpartiesTable k in GetAll())
                if (k.id == id) return k;
            return null;
        }

        public IEnumerable<CounterpartiesTable> GetAll()
        {
            List<CounterpartiesTable> list = new List<CounterpartiesTable>();
            try
            {
                using (var reader = db.execSelect("SELECT id, name, type, tax_number, phone, email FROM counterparties"))
                {
                    if (reader != null)
                        while (reader.Read())
                            list.Add(new CounterpartiesTable(
                                reader.GetInt32("id"),
                                reader.GetString("name"),
                                reader.GetString("type"),
                                reader.IsDBNull(reader.GetOrdinal("tax_number")) ? null : reader.GetString("tax_number"),
                                reader.IsDBNull(reader.GetOrdinal("phone")) ? null : reader.GetString("phone"),
                                reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString("email")
                            ));
                }
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
            return list;
        }

        public bool Add(CounterpartiesTable entity)
        {
            try 
            { 
                string tax = entity.tax_number != null ? $"'{entity.tax_number}'" : "NULL";
                string phone = entity.phone != null ? $"'{entity.phone}'" : "NULL";
                string email = entity.email != null ? $"'{entity.email}'" : "NULL";

                return db.execSql($"INSERT INTO counterparties (id, name, type, tax_number, phone, email) VALUES ({entity.id}, '{entity.name}', '{entity.type}', {tax}, {phone}, {email})"); 
            }
            catch (Exception e) { Console.WriteLine(e.Message); return false; }
        }

        public bool Delete(CounterpartiesTable entity)
        {
            try 
            { 
                return db.execSql($"DELETE FROM counterparties WHERE id = {entity.id}"); 
            }
            catch (Exception e) { Console.WriteLine(e.Message); return false; }
        }

        public bool Update(CounterpartiesTable entity)
        {
            try 
            { 
                string tax = entity.tax_number != null ? $"'{entity.tax_number}'" : "NULL";
                string phone = entity.phone != null ? $"'{entity.phone}'" : "NULL";
                string email = entity.email != null ? $"'{entity.email}'" : "NULL";

                return db.execSql($"UPDATE counterparties SET name = '{entity.name}', type = '{entity.type}', tax_number = {tax}, phone = {phone}, email = {email} WHERE id = {entity.id}"); 
            }
            catch (Exception e) { Console.WriteLine(e.Message); return false; }
        }
    }
}