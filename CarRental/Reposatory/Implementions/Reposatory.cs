using CarRental.Models.DBContext;
using CarRental.Reposatory.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CarRental.Reposatory.Implementions
{
    public class Reposatory<T> : IReposatory<T> where T : class
    {
        protected readonly CarRentalDbContext _context;
        protected readonly DbSet<T> _dbset;
        public Reposatory(CarRentalDbContext context)
        {
            _context = context; // db
            _dbset = context.Set<T>();// class 
        }
        public async Task AddAsync(T entity)
        {
            await _dbset.AddAsync(entity);
            _context.SaveChanges();
        }

        public async Task AddRange(IEnumerable<T> entities)
        {
            await _dbset.AddRangeAsync(entities);
            _context.SaveChanges();
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbset.AnyAsync(predicate);
        }

        public async Task<int> CountAsync()
        {
            return await _dbset.CountAsync();
        }

        public void Delete(T entity)
        {
            _dbset.Remove(entity);
            _context.SaveChanges();
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _dbset.RemoveRange(entities);
            _context.SaveChanges();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbset.Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbset.ToListAsync();
        }

        public async Task<T?> GetByIDAsync(int id)
        {
            return await _dbset.FindAsync(id);
        }

        public void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }

    }
}
