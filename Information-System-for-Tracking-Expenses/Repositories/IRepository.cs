using System.Collections.Generic;

namespace FinancialApp.Repositories
{
    public interface IRepository<T> where T : class
    {
        T? GetbyID(int id);
        bool Add(T entity);
        bool Delete(T entity);
        bool Update(T entity);
        IEnumerable<T> GetAll();
    }
}