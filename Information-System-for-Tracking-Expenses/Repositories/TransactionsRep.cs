using System;
using System.Collections.Generic;
using System.Globalization;
using FinancialApp.Tables;

namespace FinancialApp.Repositories
{
    internal class TransactionsRep
    {
        DB db = DB.getInstance();

        public TransactionsTable GetbyID(int id)
        {
            foreach (TransactionsTable k in GetAll())
                if (k.id == id) return k;
            return null;
        }

        public IEnumerable<TransactionsTable> GetAll()
        {
            List<TransactionsTable> list = new List<TransactionsTable>();
            try
            {
                using (var reader = db.execSelect("SELECT id, account_id, counterparty_id, employee_id, category_id, user_id, type, amount, currency, transaction_date, description, document_number FROM transactions"))
                {
                    if (reader != null)
                        while (reader.Read())
                            list.Add(new TransactionsTable(
                                reader.GetInt32("id"),
                                reader.GetInt32("account_id"),
                                reader.IsDBNull(reader.GetOrdinal("counterparty_id")) ? (int?)null : reader.GetInt32("counterparty_id"),
                                reader.IsDBNull(reader.GetOrdinal("employee_id")) ? (int?)null : reader.GetInt32("employee_id"),
                                reader.GetInt32("category_id"),
                                reader.GetInt32("user_id"),
                                reader.GetString("type"),
                                reader.GetDecimal("amount"),
                                reader.GetString("currency"),
                                reader.GetDateTime("transaction_date"),
                                reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString("description"),
                                reader.IsDBNull(reader.GetOrdinal("document_number")) ? null : reader.GetString("document_number")
                            ));
                }
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
            return list;
        }

        public bool Add(TransactionsTable entity)
        {
            try 
            { 
                string cpId = entity.counterparty_id.HasValue ? entity.counterparty_id.Value.ToString() : "NULL";
                string empId = entity.employee_id.HasValue ? entity.employee_id.Value.ToString() : "NULL";
                string amount = entity.amount.ToString(CultureInfo.InvariantCulture);
                string dateStr = entity.transaction_date.ToString("yyyy-MM-dd HH:mm:ss");
                string desc = entity.description != null ? $"'{entity.description}'" : "NULL";
                string docNum = entity.document_number != null ? $"'{entity.document_number}'" : "NULL";

                string query = $"INSERT INTO transactions (id, account_id, counterparty_id, employee_id, category_id, user_id, type, amount, currency, transaction_date, description, document_number) " +
                               $"VALUES ({entity.id}, {entity.account_id}, {cpId}, {empId}, {entity.category_id}, {entity.user_id}, '{entity.type}', {amount}, '{entity.currency}', '{dateStr}', {desc}, {docNum})";

                return db.execSql(query);
            }
            catch (Exception e) { Console.WriteLine(e.Message); return false; }
        }

        public bool Delete(TransactionsTable entity)
        {
            try 
            { 
                return db.execSql($"DELETE FROM transactions WHERE id = {entity.id}"); 
            }
            catch (Exception e) { Console.WriteLine(e.Message); return false; }
        }

        public bool Update(TransactionsTable entity)
        {
            try 
            { 
                string cpId = entity.counterparty_id.HasValue ? entity.counterparty_id.Value.ToString() : "NULL";
                string empId = entity.employee_id.HasValue ? entity.employee_id.Value.ToString() : "NULL";
                string amount = entity.amount.ToString(CultureInfo.InvariantCulture);
                string dateStr = entity.transaction_date.ToString("yyyy-MM-dd HH:mm:ss");
                string desc = entity.description != null ? $"'{entity.description}'" : "NULL";
                string docNum = entity.document_number != null ? $"'{entity.document_number}'" : "NULL";

                string query = $"UPDATE transactions SET " +
                               $"account_id = {entity.account_id}, " +
                               $"counterparty_id = {cpId}, " +
                               $"employee_id = {empId}, " +
                               $"category_id = {entity.category_id}, " +
                               $"user_id = {entity.user_id}, " +
                               $"type = '{entity.type}', " +
                               $"amount = {amount}, " +
                               $"currency = '{entity.currency}', " +
                               $"transaction_date = '{dateStr}', " +
                               $"description = {desc}, " +
                               $"document_number = {docNum} " +
                               $"WHERE id = {entity.id}";

                return db.execSql(query);
            }
            catch (Exception e) { Console.WriteLine(e.Message); return false; }
        }
    }
}