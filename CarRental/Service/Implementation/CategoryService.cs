using CarRental.Models;
using CarRental.Models.DBContext;
using CarRental.Reposatory.Interfaces;
using CarRental.Service.Interfaces;
using CarRental.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Service.Implementation
{
    public class CategoryService : Service<Category>, ICategoryService
    {
        readonly IReposatory<Category> _categoryRepo;
        readonly IReposatory<Car> carReposatory;
        readonly CarRentalDbContext _context;
        public CategoryService(IReposatory<Category> categoryRepo, IReposatory<Car> carRepo,CarRentalDbContext context)
            :base(categoryRepo)
        {
            _categoryRepo = categoryRepo;
            carReposatory = carRepo;
            _context = context;
        }
        public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
        {
            return await _categoryRepo.GetAllAsync();
        }
        public async Task<IEnumerable<CategoryWithCountVM>> GetCategoriesWithCountAsync()
        {
            return await _context.Categories
      .Select(c => new CategoryWithCountVM
      {
          CategoryID = c.CategoryId,
          CategoryName = c.CategoryName,
          Description = c.Description,
          CarCount = c.Cars.Count()
      })
      .ToListAsync();
        }
    }
}
