using CarRental.Models;
using CarRental.Models.DBContext;
using CarRental.Reposatory.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Reposatory.Implementions
{
    public class FeatureRepo : Reposatory<Feature>, IFeatureRepo
    {
        private readonly CarRentalDbContext _context;
        public FeatureRepo(CarRentalDbContext context) : base(context)
        {
            _context = context; 
        }

        public async Task<IEnumerable<FeatureWithCountVM>> GetFeaturesWithCarCount()
        {
            return await _context.Features
                .Select(f => new FeatureWithCountVM
                {
                    FeatureId = f.FeatureId,
                    FeatureName = f.FeatureName,
                    CarCount = f.CarFeatures.Count()
                }).ToListAsync();
        }
    }
}
