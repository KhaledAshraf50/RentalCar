using CarRental.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

public class CarRequest
{
    public int CarRequestId { get; set; }
    public int CarId { get; set; }

    public int UserId { get; set; }

    public string Brand { get; set; }

    public string Model { get; set; }

    public int Year { get; set; }
    [Required]

    public int CategoryId { get; set; }
    [Required]

    public int LocationId { get; set; }
    public decimal DailyPrice { get; set; }
    public string Description { get; set; }

    public string Transmission { get; set; }

    public string FuelType { get; set; }

    public int SeatingCapacity { get; set; }

    public string ImageUrl { get; set; }

    public bool IsApproved { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public User User { get; set; }
}
