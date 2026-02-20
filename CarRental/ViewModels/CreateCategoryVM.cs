using System.ComponentModel.DataAnnotations;

public class CreateCategoryVM
{
    [Required]
    [MaxLength(100)]
    public string CategoryName { get; set; }

    [MaxLength(500)]
    public string Description { get; set; }
}
