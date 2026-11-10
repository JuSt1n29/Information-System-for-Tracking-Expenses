using System;
using System.Collections.Generic;
using System.Globalization;
using InformationSystemForTrackingExpenses.Tables;

namespace InformationSystemForTrackingExpenses.Repositories
{
    internal class AccountsRep
    {
        DB db = DB.getInstance();

        public AccountsTable GetbyID(int id)
        {
            foreach (AccountsTable k in GetAll())
                if (k.id == id) return k;
            return null;
        }

        public IEnumerable<AccountsTable> GetAll()
        {
            List<AccountsTable> list = new List<AccountsTable>();
            try
            {
                using (var reader = db.execSelect("SELECT id, name, type, currency, balance FROM accounts"))
                {
                    if (reader != null)
                        while (reader.Read())
                            list.Add(new AccountsTable(
                                reader.GetInt32("id"),
                                reader.GetString("name"),
                                reader.GetString("type"),
                                reader.GetString("currency"),
                                reader.GetDecimal("balance")
                            ));
                }
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
            return list;
        }

        public bool Add(AccountsTable entity)
        {
            try 
            { 
                string balanceStr = entity.balance.ToString(CultureInfo.InvariantCulture);
                return db.execSql($"INSERT INTO accounts (id, name, type, currency, balance) VALUES ({entity.id}, '{entity.name}', '{entity.type}', '{entity.currency}', {balanceStr})"); 
            }
            catch (Exception e) { Console.WriteLine(e.Message); return false; }
        }

        public bool Delete(AccountsTable entity)
        {
            try 
            { 
                return db.execSql($"DELETE FROM accounts WHERE id = {entity.id}"); 
            }
            catch (Exception e) { Console.WriteLine(e.Message); return false; }
        }

        public bool Update(AccountsTable entity)
        {
            try 
            { 
                string balanceStr = entity.balance.ToString(CultureInfo.InvariantCulture);
                return db.execSql($"UPDATE accounts SET name = '{entity.name}', type = '{entity.type}', currency = '{entity.currency}', balance = {balanceStr} WHERE id = {entity.id}"); 
            }
            catch (Exception e) { Console.WriteLine(e.Message); return false; }
        }
    }
}