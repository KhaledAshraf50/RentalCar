using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace CarRental.Models.DBContext
{
    public class CarRentalDbContext:DbContext
    {
        public CarRentalDbContext(DbContextOptions<CarRentalDbContext> options):base(options)
        {
        }
        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<CarFeature> CarFeatures { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<Testimonial> Testimonials { get; set; }
        public DbSet<CarRequest> CarRequests { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure relationships
            // User -> Testimonial (One-to-Many)
            modelBuilder.Entity<User>()
           .HasMany(u => u.CarRequest)
           .WithOne(c => c.User)
           .HasForeignKey(c => c.UserId)
           .OnDelete(DeleteBehavior.Cascade);
            // User -> Testimonial (One-to-Many)
            modelBuilder.Entity<User>()
            .HasMany(u => u.Testimonial)
            .WithOne(t => t.User)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

            // User -> Booking (One-to-Many)
            modelBuilder.Entity<User>()
            .HasMany(u => u.Bookings)
            .WithOne(b => b.User)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);

            // User -> Booking (One-to-Many)
            modelBuilder.Entity<User>()
            .HasMany(u => u.Reviews)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

            // User -> Favorite (One-to-Many)
            modelBuilder.Entity<User>()
            .HasMany(u=>u.Favorites)
            .WithOne(f=>f.User)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Restrict);
            // User -> CarsAdded (One-to-Many)
             modelBuilder.Entity<User>()
             .HasMany(u=>u.CarsAdded)
             .WithOne(c=>c.AddedBy)
             .HasForeignKey(c=>c.AddedByUserId)
             .OnDelete(DeleteBehavior.Restrict);

            // Car -> Booking (One-to-Many)
            modelBuilder.Entity<Car>()
                .HasMany(c => c.Bookings)
                .WithOne(b => b.Car)
                .HasForeignKey(b => b.CarId)
                .OnDelete(DeleteBehavior.Restrict);
            // Car -> Review (One-to-Many)
            modelBuilder.Entity<Car>()
                .HasMany(c => c.Reviews)
                .WithOne(r => r.Car)
                .HasForeignKey(r => r.CarId)
                .OnDelete(DeleteBehavior.Cascade);
            // Car -> Favorite (One-to-Many)
            modelBuilder.Entity<Car>()
                .HasMany(c => c.Favorites)
                .WithOne(f => f.Car)
                .HasForeignKey(f => f.CarId)
                .OnDelete(DeleteBehavior.Cascade);
            // Car -> Category (Many-to-One)
            modelBuilder.Entity<Car>()
                .HasOne(c => c.Category)
                .WithMany(cat => cat.Cars)
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            // Car -> Category (Many-to-One)
            modelBuilder.Entity<Car>()
                .HasOne(c => c.Category)
                .WithMany(cat => cat.Cars)
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            // Booking -> Location (PickupLocation)
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.PickupLocation)
                .WithMany(l => l.PickupBookings)
                .HasForeignKey(b => b.PickupLocationId)
                .OnDelete(DeleteBehavior.Restrict);
            // Booking -> Location (ReturnLocation)
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.ReturnLocation)
                .WithMany(l => l.ReturnBookings)
                .HasForeignKey(b => b.ReturnLocationId)
                .OnDelete(DeleteBehavior.Restrict);
            // Booking -> Review (One-to-One)
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Review)
                .WithOne(r => r.Booking)
                .HasForeignKey<Review>(r => r.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // CarFeature (Many-to-Many between Car and Feature)
            modelBuilder.Entity<CarFeature>()
            .HasKey(cf => new { cf.CarId, cf.FeatureId });
            modelBuilder.Entity<CarFeature>()
               .HasOne(cf => cf.Car)
               .WithMany(c => c.CarFeatures)
               .HasForeignKey(cf => cf.CarId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CarFeature>()
                .HasOne(cf => cf.Feature)
                .WithMany(f => f.CarFeatures)
                .HasForeignKey(cf => cf.FeatureId)
                .OnDelete(DeleteBehavior.Cascade);
            // Unique Constraints
            modelBuilder.Entity<User>()
                .HasIndex(u=>u.Email).IsUnique();
            modelBuilder.Entity<Booking>()
                .HasIndex(b=>b.BookingReference).IsUnique();
            modelBuilder.Entity<Favorite>()
               .HasIndex(f => new { f.UserId, f.CarId })
               .IsUnique();
            //Configure decimal precision
            modelBuilder.Entity<Car>()
                .Property(c => c.DailyPrice)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalPrice)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Car>()
                .Property(c => c.AverageRating)
                .HasPrecision(3, 2);
            SeedData(modelBuilder);
        }
        void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
         new Category
         {
             CategoryId = 1,
             CategoryName = "Economy",
             Description = "Affordable and fuel-efficient cars"
         },
         new Category
         {
             CategoryId = 2,
             CategoryName = "Luxury",
             Description = "Premium and high-end vehicles"
         },
         new Category
         {
             CategoryId = 3,
             CategoryName = "SUV",
             Description = "Spacious sport utility vehicles"
         },
         new Category
         {
             CategoryId = 4,
             CategoryName = "Sports",
             Description = "High-performance sports cars"
         }
     );
            // Seed Locations
            modelBuilder.Entity<Location>().HasData(
        new Location
        {
            LocationId = 1,
            LocationName = "Downtown Office",
            Address = "123 Main Street",
            City = "San Francisco",
            IsActive = true
        },
        new Location
        {
            LocationId = 2,
            LocationName = "Airport Terminal",
            Address = "San Francisco International Airport",
            City = "San Francisco",
            IsActive = true
        },
        new Location
        {
            LocationId = 3,
            LocationName = "Los Angeles Office",
            Address = "456 Sunset Boulevard",
            City = "Los Angeles",
            IsActive = true
        },
        new Location
        {
            LocationId = 4,
            LocationName = "San Diego Branch",
            Address = "789 Pacific Highway",
            City = "San Diego",
            IsActive = true
        }
    );
            modelBuilder.Entity<Feature>().HasData(
        new Feature
        {
            FeatureId = 1,
            FeatureName = "Leather Seats",
            Icon = "sofa"
        },
        new Feature
        {
            FeatureId = 2,
            FeatureName = "Wireless Charging",
            Icon = "battery-charging"
        },
        new Feature
        {
            FeatureId = 3,
            FeatureName = "Panoramic Sunroof",
            Icon = "sun"
        },
        new Feature
        {
            FeatureId = 4,
            FeatureName = "360 Camera",
            Icon = "camera"
        }
    );
          
            // Seed Admin User
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    FullName = "Khaled Ashraf",
                    Email = "khaled@gmail.com",
                    Phone = "+201096541656",
                    PasswordHash = "$2a$11$JcY6Zx3UzLhS5Q5vW8nKj.xC1FgS9Q2M3p4R5t6Y7z8A9b0c1d2e3f4", // Password: Admin123!                    Role = "Admin",
                    Role="Admin",
                    IsActive = true,
                    CreatedAt = new DateTime(2025,1,1,0,0,0,DateTimeKind.Utc)
                }
            );
        }
    }
}
