using CarRental.Reposatory.Implementions;

namespace CarRental.Reposatory.Interfaces
{
    public interface ICarReposatoryReposatory : IReposatory<CarRequest>
    {
        public Task UpdateCarFeaturesAsync(int carId, List<int> featureIds);
        public Task<List<CarRequest>> GetPendingRequestsWithUserAsync();

    }
}
