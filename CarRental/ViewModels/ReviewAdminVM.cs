public class ReviewAdminVM
{
    public int ReviewId { get; set; }

    public string UserName { get; set; } = null!;

    public string CarName { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;

    public DateTime ReviewDate { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public bool IsApproved { get; set; }
}
