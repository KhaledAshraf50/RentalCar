using CarRental.Models;
using CarRental.Reposatory.Interfaces;
using CarRental.Service.Interfaces;

namespace CarRental.Service.Implementation
{
    public class FeatureService : Service<Feature>, IFeatureService
    {
        public FeatureService(IReposatory<Feature> repo) : base(repo)
        {
        }
    }
}
