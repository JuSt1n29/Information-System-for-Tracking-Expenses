using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using InformationSystemForTrackingExpenses.Tables;

namespace InformationSystemForTrackingExpenses.Repositories
{
    internal class UsersRep
    {
        private readonly DB db = DB.getInstance();

        public static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password ?? ""));
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        public void EnsureDefaultAdminExists()
        {
            try
            {
                if (GetByEmailOrName("root") != null) return;

                var admin = new UsersTable(
                    0,
                    "root",
                    "root@terminal.local",
                    HashPassword("root"),
                    "admin",
                    DateTime.Now,
                    null
                );
                Add(admin, isAlreadyHashed: true);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in EnsureDefaultAdminExists: " + e.Message);
            }
        }

        public UsersTable? Authenticate(string loginOrEmail, string password)
        {
            if (string.IsNullOrWhiteSpace(loginOrEmail) || string.IsNullOrEmpty(password))
                return null;

            EnsureDefaultAdminExists();

            var user = GetByEmailOrName(loginOrEmail.Trim());
            if (user == null) return null;

            string inputHash = HashPassword(password);
            if (string.Equals(user.password_hash, inputHash, StringComparison.OrdinalIgnoreCase))
                return user;

            return null;
        }

        public UsersTable? GetbyID(int id)
        {
            foreach (UsersTable k in GetAll())
                if (k.id == id) return k;
            return null;
        }

        public UsersTable? GetByEmail(string email)
        {
            foreach (UsersTable k in GetAll())
                if (string.Equals(k.email, email, StringComparison.OrdinalIgnoreCase))
                    return k;
            return null;
        }

        public UsersTable? GetByEmailOrName(string login)
        {
            foreach (UsersTable k in GetAll())
            {
                if (string.Equals(k.email, login, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(k.name, login, StringComparison.OrdinalIgnoreCase))
                    return k;
            }
            return null;
        }

        public IEnumerable<UsersTable> GetAll()
        {
            List<UsersTable> list = new List<UsersTable>();
            try
            {
                using (var reader = db.execSelect("SELECT id, name, email, password_hash, role, avatar_path, created_at FROM users"))
                {
                    if (reader != null)
                        while (reader.Read())
                            list.Add(new UsersTable(
                                reader.GetInt32("id"),
                                reader.GetString("name"),
                                reader.GetString("email"),
                                reader.GetString("password_hash"),
                                reader.GetString("role"),
                                reader.GetDateTime("created_at"),
                                reader.IsDBNull(reader.GetOrdinal("avatar_path")) ? null : reader.GetString("avatar_path")
                            ));
                }
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
            return list;
        }

        public bool Add(UsersTable entity, bool isAlreadyHashed = false)
        {
            try
            {
                int nextId = 1;
                foreach (var user in GetAll())
                {
                    if (user.id >= nextId)
                        nextId = user.id + 1;
                }

                string safeName = (entity.name ?? "").Replace("'", "''");
                string safeEmail = (entity.email ?? "").Replace("'", "''");
                string passwordToStore = isAlreadyHashed
                    ? (entity.password_hash ?? "")
                    : HashPassword(entity.password_hash ?? "");
                string safeHash = passwordToStore.Replace("'", "''");
                string safeRole = (string.IsNullOrWhiteSpace(entity.role) ? "user" : entity.role.Trim().ToLowerInvariant()).Replace("'", "''");
                string avatarSql = string.IsNullOrWhiteSpace(entity.avatar_path)
                    ? "NULL"
                    : $"'{entity.avatar_path.Replace("\\", "\\\\").Replace("'", "''")}'";
                string dateStr = entity.created_at.ToString("yyyy-MM-dd HH:mm:ss");

                string query = $"INSERT INTO users (id, name, email, password_hash, role, avatar_path, created_at) VALUES ({nextId}, '{safeName}', '{safeEmail}', '{safeHash}', '{safeRole}', {avatarSql}, '{dateStr}')";
                return db.execSql(query);
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

        public bool Update(UsersTable entity, bool hashNewPassword = false)
        {
            try
            {
                string safeName = (entity.name ?? "").Replace("'", "''");
                string safeEmail = (entity.email ?? "").Replace("'", "''");
                string safeRole = (string.IsNullOrWhiteSpace(entity.role) ? "user" : entity.role.Trim().ToLowerInvariant()).Replace("'", "''");
                string avatarSql = string.IsNullOrWhiteSpace(entity.avatar_path)
                    ? "NULL"
                    : $"'{entity.avatar_path.Replace("\\", "\\\\").Replace("'", "''")}'";

                string passwordPart = "";
                if (hashNewPassword && !string.IsNullOrEmpty(entity.password_hash))
                {
                    string safeHash = HashPassword(entity.password_hash).Replace("'", "''");
                    passwordPart = $", password_hash = '{safeHash}'";
                }

                return db.execSql($"UPDATE users SET name = '{safeName}', email = '{safeEmail}'{passwordPart}, role = '{safeRole}', avatar_path = {avatarSql} WHERE id = {entity.id}");
            }
            catch (Exception e) { Console.WriteLine(e.Message); return false; }
        }
    }
}