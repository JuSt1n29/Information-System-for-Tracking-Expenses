using System.Collections.Generic;

namespace InformationSystemForTrackingExpenses.Repositories
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