using CarRental.Models.DBContext;
using CarRental.Reposatory.Implementions;
using CarRental.Reposatory.Interfaces;
using CarRental.Service.Implementation;
using CarRental.Service.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CarRental
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var culture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            // Add DbContext
            builder.Services.AddDbContext<CarRentalDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            //Generic Reposatory
            builder.Services.AddScoped(typeof(IReposatory<>), typeof(Reposatory<>));
            //Specific Reposatory
            builder.Services.AddScoped<ICarReposatory, CarReposatory>();
            builder.Services.AddScoped<IUserRepository,UserRepository>();
            builder.Services.AddScoped<IBookingRepository,BookingRepository>();
            builder.Services.AddScoped<IFeatureRepo,FeatureRepo>();
            builder.Services.AddScoped<IReviewRepository,ReviewRepository>();
            builder.Services.AddScoped<IFavoriteRepository, FavoriteRepository>();
            builder.Services.AddScoped<ITestimonialReposatory, TestimonialReposatory>();
            builder.Services.AddScoped<ICarReposatoryReposatory, CarRequestReposatory>();

            // Add Service
            builder.Services.AddScoped<ICarService, CarService>();
            builder.Services.AddScoped<IUserService,UserService>();
            builder.Services.AddScoped<IBookingService,BookingService>();
            builder.Services.AddScoped<ICategoryService,CategoryService>();
            builder.Services.AddScoped<ILocationService,LocationService>();
            builder.Services.AddScoped<IFeatureService,FeatureService>();
            builder.Services.AddScoped<IFavoriteService, FavoriteService>();
            builder.Services.AddScoped<ICarRequestService, CarRequestService>();

            // Add Seesion
            //builder.Services.AddSession(options =>
            //{
            //    options.IdleTimeout = TimeSpan.FromMinutes(120);
            //    options.Cookie.HttpOnly = true;
            //    options.Cookie.IsEssential = true;
            //});
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/HomePage/Index";
                    options.ExpireTimeSpan = TimeSpan.FromDays(30);
                });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseHttpsRedirection();
            app.MapStaticAssets();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();


            //app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=HomePage}/{action=Index}/{id?}")
                .WithStaticAssets();


            app.Run();
        }
    }
}
