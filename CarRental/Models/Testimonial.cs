using CarRental.Models;

public class Testimonial
{
    public int TestimonialId { get; set; }

    public int UserId { get; set; }

    public string Comment { get; set; } = null!;

    public int Rating { get; set; } // من 1 لـ 5

    public bool IsApproved { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public User User { get; set; }
}
