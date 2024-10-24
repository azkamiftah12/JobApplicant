using JobApplicant.Models;
using Microsoft.EntityFrameworkCore;

namespace JobApplicant.Services
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Applicant> applicants { get; set; }
    }
}
