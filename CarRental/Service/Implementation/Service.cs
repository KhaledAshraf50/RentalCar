using CarRental.Reposatory.Interfaces;
using CarRental.Service.Interfaces;

namespace CarRental.Service.Implementation
{
    public class Service<T> : IService<T> where T : class
    {
        protected readonly IReposatory<T> _repo;
        public Service(IReposatory<T> repo)
        {
            _repo=repo;
        }
        public async Task<T> CreateAsync(T entity)
        {
             await _repo.AddAsync(entity);
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _repo.GetByIDAsync(id);
            if (entity == null) return false;
            _repo.Delete(entity);
            return true;
        }

        public async Task<IEnumerable<T>> GetAllAsunc()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _repo.GetByIDAsync(id);
        }

        public async Task<T> UpdateAsync(int id, T entity)
        {
            return await Task.Run(() =>
            {
                _repo.Update(entity);
                return entity;
            });
        }
    }
}
