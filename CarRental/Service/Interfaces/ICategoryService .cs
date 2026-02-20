using CarRental.Models;
using CarRental.ViewModels;

namespace CarRental.Service.Interfaces
{
    public interface ICategoryService:IService<Category>
    {
        Task<IEnumerable<Category>> GetActiveCategoriesAsync();
        public Task<IEnumerable<CategoryWithCountVM>> GetCategoriesWithCountAsync();
    }
}
