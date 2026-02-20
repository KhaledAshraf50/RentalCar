namespace CarRental.ViewModels
{
    public class LocationsWithCountVM
    {
        public int LocationId { get; set; }
        public string? LocationName { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public int CarCount { get; set; }
        public bool IsActive { get; set; }
    }
}
