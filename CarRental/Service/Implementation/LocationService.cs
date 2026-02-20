using CarRental.Models;
using CarRental.Models.DBContext;
using CarRental.Reposatory.Implementions;
using CarRental.Reposatory.Interfaces;
using CarRental.Service.Interfaces;
using CarRental.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Service.Implementation
{
    public class LocationService : Service<Location>, ILocationService
    {
        readonly IReposatory<Location> _locationRepo;
        readonly IReposatory<Car> carReposatory;
        readonly CarRentalDbContext _context;
        public LocationService(IReposatory<Location> locationRepo, IReposatory<Car> carReposatory,CarRentalDbContext context)
            : base(locationRepo)
        {
            _locationRepo = locationRepo;
            this.carReposatory = carReposatory;
            _context = context;
            
        }
        public async Task<IEnumerable<LocationsWithCountVM>> GetActiveLocationsAsync()
        {
            return await _context.Locations
              .Select(l => new LocationsWithCountVM
              {
                  LocationName = l.LocationName,
                  Address = l.Address,
                  City = l.City,
                  IsActive=l.IsActive,
                  CarCount = l.Cars.Count(),
                  LocationId = l.LocationId
              }).Where(l=>l.IsActive)
              .ToListAsync();
        }
        public async Task<IEnumerable<LocationsWithCountVM>> GetActiveLocationsViewAsync()
        {
            return await _context.Locations
              .Select(l => new LocationsWithCountVM
              {
                  LocationName = l.LocationName,
                  Address = l.Address,
                  City = l.City,
                  IsActive = l.IsActive,
                  CarCount = l.Cars.Count(),
                  LocationId = l.LocationId
              })
              .ToListAsync();
        }
    }
}
