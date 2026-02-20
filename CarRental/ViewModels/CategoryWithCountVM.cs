using System.Reflection.Metadata.Ecma335;

namespace CarRental.ViewModels
{
    public class CategoryWithCountVM
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public int CarCount { get; set; }
    }
}
