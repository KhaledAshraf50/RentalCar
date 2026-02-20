using CarRental.Models;
using CarRental.Reposatory.Implementions;
using CarRental.Reposatory.Interfaces;
using CarRental.Service.Interfaces;
using CarRental.ViewModels;

namespace CarRental.Service.Implementation
{
    public class CarService:Service<Car>,ICarService
    {
        private readonly ICarReposatory _carRepository;
        private readonly IReposatory<Category> _categoryRepository;
        private readonly IReposatory<Location> _locationRepository;
        private readonly IReposatory<Feature> _featureRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CarService(
              ICarReposatory carRepository,
              IReposatory<Category> categoryRepository,
              IReposatory<Location> locationRepository,
              IReposatory<Feature> featureRepository,
              IWebHostEnvironment webHostEnvironment,
              IReviewRepository reviewRepository) : base(carRepository)
        {
            _carRepository = carRepository;
            _categoryRepository = categoryRepository;
            _locationRepository = locationRepository;
            _webHostEnvironment = webHostEnvironment;
            _featureRepository = featureRepository;
            _reviewRepository = reviewRepository;
        }
        public async Task<IEnumerable<Car>> GetAllCarsAsync()
        {
            return await _carRepository.GetAllCarsAsync();
        }

        public async Task<Car?> GetCarByIdAsync(int id)
        {
            return await _carRepository.GetCarWithDetailsAsync(id);
        }

        public async Task<Car?> GetCarWithDetailsAsync(int id)
        {
            return await _carRepository.GetCarWithDetailsAsync(id);
        }
        public async Task<IEnumerable<Car>> GetAvailableCarsAsync()
        {
            return await _carRepository.GetAvailableCarAsync();
        }

        public async Task<IEnumerable<Car>> GetFeaturedCarsAsync(int count = 6)
        {
            return await _carRepository.GetFeaturedCarsAsync(count);
        }

        public async Task<IEnumerable<Car>> GetCarsByCategoryAsync(int categoryId)
        {
            return await _carRepository.GetCarByCategoryAsync(categoryId);
        }

        public async Task<IEnumerable<Car>> GetCarsByLocationAsync(int locationId)
        {
            return await _carRepository.GetCarByLocationAsync(locationId);
        }

        public async Task<IEnumerable<Car>> SearchCarsAsync(string? brand, string? model, int? categoryId, int? locationId)
        {
            var cars = await _carRepository.GetAvailableCarAsync();
            if (!string.IsNullOrEmpty(brand))
                cars = cars.Where(c => c.Brand.Contains(brand, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(model))
                cars = cars.Where(c => c.Model.Contains(model, StringComparison.OrdinalIgnoreCase));
            if(categoryId.HasValue && categoryId>0)
                cars = cars.Where(c=>c.CategoryId == categoryId.Value);
            if(locationId.HasValue && locationId>0)
                cars= cars.Where(c=>c.LocationId == locationId.Value);
            return cars;
        }
        public async Task<Car> CreateCarForPendingAsync(CarViewModel model)
        {
            //validate category and location
            var category = await _categoryRepository.GetByIDAsync(model.CategoryId);
            if (category == null) throw new ArgumentException("Invalid Category");
            var location = await _locationRepository.GetByIDAsync(model.LocationId);
            if (location == null) throw new ArgumentException("Invalid Location");
            //Handle Image upload
            //create Car
            var car = new Car
            {
                Brand = model.Brand,
                Model = model.Model,
                Year = model.Year,
                DailyPrice = model.DailyPrice,
                Description = model.Description,
                Transmission = model.Transmission,
                FuelType = model.FuelType,
                SeatingCapacity = model.SeatingCapacity,
                ImageUrl = model.ImageUrl,
                CategoryId = model.CategoryId,
                LocationId = model.LocationId,
                AddedByUserId = 1,//TODO: Get From current user
                IsAvailable = true,
                IsActive = true,
                IsApproved = true,
                CreatedAt = DateTime.UtcNow
            };
            await _carRepository.AddAsync(car);
            // add feature if any
            if (model.FeatureIds.Any())
            {
                await _carRepository.UpdateCarFeaturesAsync(car.CarId, model.FeatureIds);
            }
            return car;
        }
        public async Task<Car> CreateCarAsync(CarViewModel model)
        {
            //validate category and location
            var category = await _categoryRepository.GetByIDAsync(model.CategoryId);
            if (category == null) throw new ArgumentException("Invalid Category");
            var location = await _locationRepository.GetByIDAsync(model.LocationId);
            if (location == null) throw new ArgumentException("Invalid Location");
                string imageUrl = await UploadImageAsync(model.ImageFile);
            //Handle Image upload
            //create Car
            var car = new Car
            {
                Brand = model.Brand,
                Model = model.Model,
                Year = model.Year,
                DailyPrice = model.DailyPrice,
                Description = model.Description,
                Transmission = model.Transmission,
                FuelType = model.FuelType,
                SeatingCapacity = model.SeatingCapacity,
                ImageUrl = imageUrl,
                CategoryId = model.CategoryId,
                LocationId = model.LocationId,
                AddedByUserId = 1,//TODO: Get From current user
                IsAvailable = true,
                IsActive = true,
                IsApproved = true,
                CreatedAt = DateTime.UtcNow
            };
            await _carRepository.AddAsync(car);
            // add feature if any
            if (model.FeatureIds.Any())
            {
                await _carRepository.UpdateCarFeaturesAsync(car.CarId,model.FeatureIds);
            }
            return car;
        }

        public async Task<Car> UpdateCarAsync(int id, CarViewModel model)
        {
            var car = await _carRepository.GetByIDAsync(id);
            if (car == null) throw new ArgumentException("Car Not Found");
            //update properties
            car.Brand = model.Brand;
            car.Model = model.Model;
            car.Year = model.Year;
            car.DailyPrice = model.DailyPrice;
            car.Description = model.Description;
            car.Transmission = model.Transmission;
            car.FuelType = model.FuelType;
            car.SeatingCapacity= model.SeatingCapacity;
            car.CategoryId= model.CategoryId;
            car.LocationId= model.LocationId;
            //Handle Image Update
            if(model.ImageFile != null)
            {
                car.ImageUrl = await UploadImageAsync(model.ImageFile);
            }
            //update features
            if (model.FeatureIds.Any())
            {
                await _carRepository.UpdateCarFeaturesAsync(id,model.FeatureIds);
            }
            _carRepository.Update(car);
            return car;
        }

        public async Task<bool> DeleteCarAsync(int id)
        {
            var car = await _carRepository.GetByIDAsync(id);
            if(car == null) return false;
            // Soft delete 
            if(car.IsAvailable)
            car.IsAvailable = false;
            else
                car.IsAvailable = true;
            _carRepository.Update(car);
            return true;
        }
        public async Task<bool> ToggleCarAvailabilityAsync(int id)
        {
            var car = await _carRepository.GetByIDAsync(id);
            if (car == null) return false;
            car.IsAvailable=!car.IsAvailable;
            _carRepository.Update(car);
            return car.IsAvailable;
        }

        public async Task<bool> UpdateCarFeaturesAsync(int carId, List<int> FeatureIds)
        {
            await _carRepository.UpdateCarFeaturesAsync(carId, FeatureIds);
            return true;
        }

        public async Task<decimal> CalculateRentalPriceAsync(int carId, DateTime pickupDate, DateTime returnDate)
        {
            var car = await _carRepository.GetByIDAsync(carId);
            if (car == null) throw new ArgumentException("Car not found");
            var days = (returnDate - pickupDate).Days;
            if(days < 1) days = 1;
            return car.DailyPrice * days;

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
            using(var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }
            return $"/uploads/cars/{uniqueFileName}";
        }

        public async Task<int> CountAsync()
        {
            return await _carRepository.CountAsync();
        }

        public Task AddReviewAsync(Review review)
        {
           return _reviewRepository.AddReviewAsync(review);
        }

        public Task<bool> HasUserBookedCarAsync(int userId, int carId)
        {
            return _reviewRepository.HasUserBookedCarAsync(userId, carId);
        }

        public Task<bool> HasUserReviewedCarAsync(int userId, int carId)
        {
            return _reviewRepository.HasUserReviewedCarAsync(userId, carId);
        }

        public Task<int?> GetCompletedBookingIdAsync(int userId, int carId)
        {
            return _reviewRepository.GetCompletedBookingIdAsync(userId, carId);
        }
    }
}
