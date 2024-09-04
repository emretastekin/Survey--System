using Microsoft.EntityFrameworkCore;
using SurveyApp.Models;

namespace SurveyApp.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        public DbSet<Survey>? Surveys { get; set; }

    }
}
