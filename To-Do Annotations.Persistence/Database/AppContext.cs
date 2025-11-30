using Microsoft.EntityFrameworkCore;
using To_Do_Annonations.Domain.Entities;

namespace To_Do_Annotations.Persistence.Database
{
    public class AppContext : DbContext
    {
        public AppContext(DbContextOptions<AppContext> options) : base(options)
        {
        }
        public DbSet<ToDoTask> Tasks { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
