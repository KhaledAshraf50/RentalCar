using CarRental.Models;
using CarRental.Reposatory.Implementions;
using CarRental.Reposatory.Interfaces;
using CarRental.Service.Interfaces;
using CarRental.ViewModels;
using Microsoft.AspNetCore.Hosting;

namespace CarRental.Service.Implementation
{
    public class CarRequestService : Service<CarRequest>, ICarRequestService
    {
        private readonly IReposatory<CarRequest> _repo;
        private readonly ICarService _carService;
        private readonly IReposatory<Category> _categoryRepository;
        private readonly IReposatory<Location> _locationRepository;
        private readonly IReposatory<Feature> _featureRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICarReposatoryReposatory _carRequestReposatory;



        public CarRequestService(IReposatory<CarRequest> repo
            ,ICarService carService
            ,IReposatory<Category> categoryRepository
            , IReposatory<Location> locationRepository
            , IReposatory<Feature> featureRepository
            , IWebHostEnvironment webHostEnvironment
            , ICarReposatoryReposatory carRequestReposatory) : base(repo)
        {
            _repo = repo;
            _carService = carService;
            _categoryRepository = categoryRepository;
            _locationRepository = locationRepository;
            _featureRepository = featureRepository;
            _webHostEnvironment = webHostEnvironment;
            _carRequestReposatory = carRequestReposatory;
        }
        public async Task<CarRequest> CreateCarRequestAsync(CarViewModel model,int userId)
        {
            //validate category and location
            var category = await _categoryRepository.GetByIDAsync(model.CategoryId);
            if (category == null) throw new ArgumentException("Invalid Category");
            var location = await _locationRepository.GetByIDAsync(model.LocationId);
            if (location == null) throw new ArgumentException("Invalid Location");
            //Handle Image upload
            string imageUrl = await UploadImageAsync(model.ImageFile);
            //create Car
            var request = new CarRequest
            {
                UserId = userId,
                CarId = model.CarId,
                Brand = model.Brand,
                Model = model.Model,
                Year = model.Year,
                DailyPrice = model.DailyPrice,
                Description = model.Description,
                Transmission = model.Transmission,
                FuelType = model.FuelType,
                SeatingCapacity = model.SeatingCapacity,
                CategoryId = model.CategoryId,
                LocationId = model.LocationId,
                ImageUrl = imageUrl,
                IsApproved = false,
                CreatedAt = DateTime.Now
            };
            await _carRequestReposatory.AddAsync(request);
            // add feature if any
            if (model.FeatureIds.Any())
            {
                await _carRequestReposatory.UpdateCarFeaturesAsync(request.CarRequestId, model.FeatureIds);
            }
            return request;
        }
        public async Task ApproveRequestAsync(int id)
        {
            var request = await _repo.GetByIDAsync(id);
            if (request == null) return;
            var carVm = new CarViewModel
            {
                CarId = request.CarId,
                Brand = request.Brand,
                Model = request.Model,
                Year = request.Year,
                DailyPrice = request.DailyPrice,
                Description = request.Description,
                Transmission = request.Transmission,
                FuelType = request.FuelType,
                SeatingCapacity = request.SeatingCapacity,
                LocationId = request.LocationId,
                CategoryId = request.CategoryId,
                ImageUrl = request.ImageUrl
            };
            await _carService.CreateCarForPendingAsync(carVm);

            request.IsApproved = true;
            _repo.Update(request);
        }

        public async Task<List<CarRequest>> GetPendingRequestsAsync()
        {
            return await _carRequestReposatory.GetPendingRequestsWithUserAsync();
        }

        async Task<string> UploadImageAsync(IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return "/imgs/default-car.jpg";
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "cars");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }
            return $"/uploads/cars/{uniqueFileName}";
        }

   
    }
}
