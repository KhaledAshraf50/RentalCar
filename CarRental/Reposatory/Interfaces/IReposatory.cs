using System.Linq.Expressions;

namespace CarRental.Reposatory.Interfaces
{
    public interface IReposatory<T> where T : class
    {
        //Get All
        Task<IEnumerable<T>> GetAllAsync();
        //Get By Id
        Task<T?> GetByIDAsync(int id);

        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        Task AddAsync(T entity);
        Task AddRange(IEnumerable<T> entities);

        void Update(T entity);

        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
        Task<int> CountAsync();
        //Any
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        
    }
}
