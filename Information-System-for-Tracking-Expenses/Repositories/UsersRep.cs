using System;
using System.Collections.Generic;
using InformationSystemForTrackingExpenses.Tables;

namespace InformationSystemForTrackingExpenses.Repositories
{
    internal class UsersRep
    {
        DB db = DB.getInstance();

        public UsersTable GetbyID(int id)
        {
            foreach (UsersTable k in GetAll())
                if (k.id == id) return k;
            return null;
        }

        public UsersTable GetByEmail(string email)
        {
            foreach (UsersTable k in GetAll())
                if (string.Equals(k.email, email, StringComparison.OrdinalIgnoreCase)) 
                    return k;
            return null;
        }

        public IEnumerable<UsersTable> GetAll()
        {
            List<UsersTable> list = new List<UsersTable>();
            try
            {
                using (var reader = db.execSelect("SELECT id, name, email, password_hash, role, created_at FROM users"))
                {
                    if (reader != null)
                        while (reader.Read())
                            list.Add(new UsersTable(
                                reader.GetInt32("id"),
                                reader.GetString("name"),
                                reader.GetString("email"),
                                reader.GetString("password_hash"),
                                reader.GetString("role"),
                                reader.GetDateTime("created_at")
                            ));
                }
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
            return list;
        }

        public bool Add(UsersTable entity)
        {
            try 
            { 
                // Вычисляем следующий свободный id прямо в репозитории
                int nextId = 1;
                foreach (var user in GetAll())
                {
                    if (user.id >= nextId) 
                        nextId = user.id + 1;
                }

                string dateStr = entity.created_at.ToString("yyyy-MM-dd HH:mm:ss");
                
                return db.execSql($"INSERT INTO users (id, name, email, password_hash, role, created_at) VALUES ({nextId}, '{entity.name}', '{entity.email}', '{entity.password_hash}', '{entity.role}', '{dateStr}')"); 
            }
            catch (Exception e) { Console.WriteLine(e.Message); return false; }
        }

        public bool Delete(UsersTable entity)
        {
            try 
            { 
                return db.execSql($"DELETE FROM users WHERE id = {entity.id}"); 
            }
            catch (Exception e) { Console.WriteLine(e.Message); return false; }
        }

        public bool Update(UsersTable entity)
        {
            try 
            { 
                return db.execSql($"UPDATE users SET name = '{entity.name}', email = '{entity.email}', password_hash = '{entity.password_hash}', role = '{entity.role}' WHERE id = {entity.id}"); 
            }
            catch (Exception e) { Console.WriteLine(e.Message); return false; }
        }
    }
}