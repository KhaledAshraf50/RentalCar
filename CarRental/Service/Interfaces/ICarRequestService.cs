using CarRental.ViewModels;

namespace CarRental.Service.Interfaces
{
    public interface ICarRequestService :IService<CarRequest>
    {
        Task<List<CarRequest>> GetPendingRequestsAsync();
        Task ApproveRequestAsync(int id);
        public Task<CarRequest> CreateCarRequestAsync(CarViewModel model, int userId);

    }
}
