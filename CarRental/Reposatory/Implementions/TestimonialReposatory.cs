using CarRental.Models.DBContext;
using CarRental.Reposatory.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Reposatory.Implementions
{
    public class TestimonialReposatory : Reposatory<Testimonial>, ITestimonialReposatory
    {
        readonly CarRentalDbContext _context;
        IUserRepository _IuserRepository;
        public TestimonialReposatory(CarRentalDbContext context,
            IUserRepository userRepository) : base(context)
        {
            _context = context;
            _IuserRepository = userRepository;
        }

        public async Task AddTestimonialAsync(int userId, string comment, int rating)
        {
            var user = _IuserRepository.GetByIDAsync(userId);
            if(user == null)
            {
                throw new ArgumentException("User Not Found");
            }
            var testimonial = new Testimonial
            {
                UserId = userId,
                Comment = comment,
                Rating = rating,
                IsApproved = false,
                CreatedAt = DateTime.UtcNow,
            };
            _context.Testimonials.Add(testimonial);
            await _context.SaveChangesAsync();

        }

        public async Task ApproveTestimonialAsync(int testimonialId)
        {
            var testimonial = _context.Testimonials
                .FirstOrDefault(t=>t.TestimonialId == testimonialId);
            if (testimonial == null)
                return;
            testimonial.IsApproved = true;
            await _context.SaveChangesAsync();
        }

        public async Task<List<Testimonial>> GetApprovedTestimonialsAsync()
        {
            return await _context.Testimonials
                .Include(t=>t.User)
                .Where(t=>t.IsApproved)
                .OrderByDescending(t=>t.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Testimonial>> GetPendingTestimonialsAsync()
        {
            return await _context.Testimonials
                .Include(t => t.User)
                .Where(t => !t.IsApproved)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task RemoveTestimonialAsync(int userId)
        {
            var testimonial = _context.Testimonials
                .Where(t => t.UserId == userId).FirstOrDefault();
            _context.Testimonials.Remove(testimonial);
           await _context.SaveChangesAsync();
        }
    }
}
