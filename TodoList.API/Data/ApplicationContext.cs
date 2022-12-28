using Microsoft.EntityFrameworkCore;
using ToDoListApi.Dtos;

namespace ToDoListApi.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Todo> Todos { get; set; }


        private IConfiguration _config { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options,IConfiguration config) : base(options)
        {
            Database.EnsureCreated();
            _config = config;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Todo>()
                .HasOne(t => t.user)
                .WithMany(u => u.Todos);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Todos)
                .WithOne(t => t.user);
        }
    }
}
