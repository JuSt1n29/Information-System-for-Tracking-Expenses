using System;
using System.Collections.Generic;
using System.Globalization;
using InformationSystemForTrackingExpenses.Tables;

namespace InformationSystemForTrackingExpenses.Repositories
{
    public class TransactionsRep
    {
        private DB db = DB.getInstance();

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
                string query = @"SELECT 
                    transaction_id, transaction_date, transaction_type, amount, currency, document_number, description,
                    account_name, account_type,
                    category_name, category_type,
                    counterparty_name, counterparty_type, counterparty_tax_number,
                    employee_name, employee_position,
                    user_name, user_email
                FROM v_transactions_catalog";

                using (var reader = db.execSelect(query))
                {
                    if (reader != null)
                        while (reader.Read())
                        {
                            list.Add(new TransactionsTable(
                                reader.GetInt32("transaction_id"),
                                reader.GetDateTime("transaction_date"),
                                reader.GetString("transaction_type"),
                                reader.GetDecimal("amount"),
                                reader.GetString("currency"),
                                reader.IsDBNull(reader.GetOrdinal("document_number")) ? "" : reader.GetString("document_number"),
                                reader.IsDBNull(reader.GetOrdinal("description")) ? "" : reader.GetString("description"),
                                reader.IsDBNull(reader.GetOrdinal("account_name")) ? null : reader.GetString("account_name"),
                                reader.IsDBNull(reader.GetOrdinal("account_type")) ? null : reader.GetString("account_type"),
                                reader.IsDBNull(reader.GetOrdinal("category_name")) ? null : reader.GetString("category_name"),
                                reader.IsDBNull(reader.GetOrdinal("category_type")) ? null : reader.GetString("category_type"),
                                reader.IsDBNull(reader.GetOrdinal("counterparty_name")) ? null : reader.GetString("counterparty_name"),
                                reader.IsDBNull(reader.GetOrdinal("counterparty_type")) ? null : reader.GetString("counterparty_type"),
                                reader.IsDBNull(reader.GetOrdinal("counterparty_tax_number")) ? null : reader.GetString("counterparty_tax_number"),
                                reader.IsDBNull(reader.GetOrdinal("employee_name")) ? null : reader.GetString("employee_name"),
                                reader.IsDBNull(reader.GetOrdinal("employee_position")) ? null : reader.GetString("employee_position"),
                                reader.IsDBNull(reader.GetOrdinal("user_name")) ? null : reader.GetString("user_name"),
                                reader.IsDBNull(reader.GetOrdinal("user_email")) ? null : reader.GetString("user_email")
                            ));
                        }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in TransactionsRep.GetAll: " + e.Message);
            }
            return list;
        }

        public bool Add(TransactionsTable entity)
        {
            try
            {
                string amountStr = entity.amount.ToString(CultureInfo.InvariantCulture);
                string dateStr = entity.transaction_date.ToString("yyyy-MM-dd HH:mm:ss");
                string desc = (entity.description ?? "").Replace("'", "''");
                string docNum = (entity.document_number ?? "").Replace("'", "''");
                string type = (entity.type ?? "expense").Replace("'", "''");
                string currency = (entity.currency ?? "UAH").Replace("'", "''");

                string counterpartySql = entity.counterparty_id.HasValue ? entity.counterparty_id.Value.ToString() : "NULL";
                string employeeSql = entity.employee_id.HasValue ? entity.employee_id.Value.ToString() : "NULL";

                string query =
                    "INSERT INTO transactions (id, account_id, counterparty_id, employee_id, category_id, user_id, type, amount, currency, transaction_date, description, document_number) " +
                    $"VALUES ({entity.id}, {entity.account_id}, {counterpartySql}, {employeeSql}, {entity.category_id}, {entity.user_id}, '{type}', {amountStr}, '{currency}', '{dateStr}', '{desc}', '{docNum}')";

                return db.execSql(query);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in TransactionsRep.Add: " + e.Message);
                return false;
            }
        }

        public bool Delete(TransactionsTable entity)
        {
            try
            {
                return db.execSql($"DELETE FROM transactions WHERE id = {entity.id}");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in TransactionsRep.Delete: " + e.Message);
                return false;
            }
        }

        public bool Update(TransactionsTable entity)
        {
            try
            {
                string amountStr = entity.amount.ToString(CultureInfo.InvariantCulture);
                string desc = (entity.description ?? "").Replace("'", "''");

                string query = $"UPDATE transactions SET amount = {amountStr}, description = '{desc}' WHERE id = {entity.id}";
                return db.execSql(query);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in TransactionsRep.Update: " + e.Message);
                return false;
            }
        }
    }
}