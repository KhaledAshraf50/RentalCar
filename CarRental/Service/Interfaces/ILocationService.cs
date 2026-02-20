using CarRental.Models;
using CarRental.Service.Implementation;
using CarRental.ViewModels;

namespace CarRental.Service.Interfaces
{
    public interface ILocationService:IService<Location>
    {
        Task<IEnumerable<LocationsWithCountVM>> GetActiveLocationsAsync();
         Task<IEnumerable<LocationsWithCountVM>> GetActiveLocationsViewAsync();

    }
}
