using System;
using System.Collections.Generic;
using InformationSystemForTrackingExpenses.Tables;

namespace InformationSystemForTrackingExpenses.Repositories
{
    public class CategoriesRep
    {
        private DB db = DB.getInstance(); // Используем singleton, как у тебя заведено в DB.cs

        public List<CategoriesTable> GetAll()
        {
            var list = new List<CategoriesTable>();
            try
            {
                string query = "SELECT id, name, type FROM categories";
                using (var reader = db.execSelect(query))
                {
                    while (reader != null && reader.Read())
                    {
                        list.Add(new CategoriesTable(
                            reader.GetInt32("id"),
                            reader.GetString("name"),
                            reader.GetString("type")
                        ));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in CategoriesRep.GetAll: " + e.Message);
            }
            return list;
        }

        public bool Add(CategoriesTable item)
        {
            string query = $"INSERT INTO categories (id, name, type) VALUES ({item.id}, '{item.name}', '{item.type}')";
            return db.execSql(query); // Используем execSql
        }

        public bool Update(CategoriesTable item)
        {
            string query = $"UPDATE categories SET name='{item.name}', type='{item.type}' WHERE id={item.id}";
            return db.execSql(query); // Используем execSql
        }

        public bool Delete(CategoriesTable item)
        {
            string query = $"DELETE FROM categories WHERE id={item.id}";
            return db.execSql(query); // Используем execSql
        }
    }
}