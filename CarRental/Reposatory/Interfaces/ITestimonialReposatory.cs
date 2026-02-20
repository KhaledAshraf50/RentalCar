namespace CarRental.Reposatory.Interfaces
{
    public interface ITestimonialReposatory : IReposatory<Testimonial>
    {
        Task<List<Testimonial>> GetApprovedTestimonialsAsync();
        Task AddTestimonialAsync(int userId, string comment, int rating);
        Task ApproveTestimonialAsync(int testimonialId);
        Task<List<Testimonial>> GetPendingTestimonialsAsync();
        Task RemoveTestimonialAsync(int userId);

    }
}
