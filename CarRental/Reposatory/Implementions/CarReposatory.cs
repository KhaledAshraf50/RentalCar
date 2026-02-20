using CarRental.Models;
using CarRental.Models.DBContext;
using CarRental.Reposatory.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Reposatory.Implementions
{
    public class CarReposatory : Reposatory<Car>, ICarReposatory
    {
        public CarReposatory(CarRentalDbContext context):base(context)
        {
            
        }
        public async Task<IEnumerable<Car>> GetAvailableCarAsync()
        {
            return await _context.Cars
                .Include(c=>c.Category)
                .Include(c=>c.Location)
                .Include(c=>c.CarFeatures)
                .ThenInclude(cf=>cf.Feature)
                .Where(c=>c.IsAvailable&& c.IsActive && c.IsApproved)
                .ToListAsync();
        }

        public async Task<IEnumerable<Car>> GetCarByCategoryAsync(int categoryId)
        {
            return await _context.Cars
                .Include(c=>c.Category)
                .Include(c=>c.Location)
                .Where(c=>c.CategoryId == categoryId && c.IsActive && c.IsApproved)
                .ToListAsync();
        }

        public async Task<IEnumerable<Car>> GetCarByLocationAsync(int locationId)
        {
            return await _context.Cars
                .Include(c=>c.Category)
                .Include(c=>c.Location)
                .Where(c=>c.LocationId == locationId && c.IsActive && c.IsApproved)
                .ToListAsync();
        }
        public async Task<IEnumerable<Car>> GetFeaturedCarsAsync(int count = 6)
        {
            return await _context.Cars
                .Include(c => c.Category)
                .Include(c => c.Location)
                .Include(c => c.CarFeatures)
                .ThenInclude(cf => cf.Feature)
                .Where(c => c.IsAvailable && c.IsActive && c.IsApproved)
                .OrderByDescending(c => c.AverageRating ?? 0)
                .Take(count)
                .ToListAsync();
        }
        public async Task<Car?> GetCarWithDetailsAsync(int id)
        {
            return await _context.Cars
                .Include(c => c.Category)
                .Include(c=>c.Location)
                .Include (c=>c.CarFeatures)
                .ThenInclude(cf=>cf.Feature)
                .Include(c=>c.Reviews.Where(r=>r.IsApproved))
                .ThenInclude(r=>r.User)
                .FirstOrDefaultAsync(c=>c.CarId == id && c.IsActive && c.IsApproved);
        }
        public async Task UpdateCarFeaturesAsync(int carId, List<int> featureIds)
        {
            // remove existing feature
            var existingFeatures = await _context.CarFeatures
                .Where(cf=>cf.CarId == carId).ToListAsync();
            _context.CarFeatures.RemoveRange(existingFeatures);
            //Add New Feature
            var newFeature = featureIds.Select(featureId => new CarFeature
            {
                CarId = carId,
                FeatureId = featureId
            });
            await _context.CarFeatures.AddRangeAsync(newFeature);
            _context.SaveChanges();
        }

        public async Task<IEnumerable<Car>> GetAllCarsAsync()
        {
            return await _context.Cars
                           .Include(c => c.Category)
                           .Include(c => c.Location)
                           .Include(c => c.CarFeatures)
                           .ThenInclude(cf => cf.Feature)
                           .Where(c => c.IsActive && c.IsApproved)
                           .ToListAsync();
        }
    }
}
