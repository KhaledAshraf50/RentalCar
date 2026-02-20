using CarRental.Models;

namespace CarRental.Reposatory.Interfaces
{
    public interface IFeatureRepo:IReposatory<Feature>
    {
         Task<IEnumerable<FeatureWithCountVM>> GetFeaturesWithCarCount();
    }
}
