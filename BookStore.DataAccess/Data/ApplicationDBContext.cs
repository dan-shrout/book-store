using BookStore.Models;
using Microsoft.EntityFrameworkCore;
namespace BookStore.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Adding data seeding for the Category entity
            modelBuilder.Entity<Category>().HasData(
                    new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
                    new Category { Id = 2, Name = "Comedy", DisplayOrder = 2 },
                    new Category { Id = 3, Name = "Drama", DisplayOrder = 3 },
                    new Category { Id = 4, Name = "Fantasy", DisplayOrder = 4 },
                    new Category { Id = 5, Name = "Horror", DisplayOrder = 5 }
                );
        }
    }
}