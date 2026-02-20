using CarRental.Models;
using CarRental.Models.DBContext;
using CarRental.Reposatory.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Reposatory.Implementions
{
    public class CarRequestReposatory : Reposatory<CarRequest> , ICarReposatoryReposatory
    {
        private readonly DbSet<CarRequest> _dbset;
        public CarRequestReposatory(CarRentalDbContext context) : base(context)
        {
            _dbset = context.Set<CarRequest>();
        }
        public async Task UpdateCarFeaturesAsync(int carId, List<int> featureIds)
        {
            // remove existing feature
            var existingFeatures = await _context.CarFeatures
                .Where(cf => cf.CarId == carId).ToListAsync();
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
        public async Task<List<CarRequest>> GetPendingRequestsWithUserAsync()
        {
            return await _dbset
                .Include(r => r.User)
                .Where(r => !r.IsApproved)
                .ToListAsync();
        }

    }
}
