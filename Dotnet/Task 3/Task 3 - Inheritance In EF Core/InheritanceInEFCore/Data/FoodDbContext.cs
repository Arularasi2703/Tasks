using InheritanceInEFCore.Models;
using Microsoft.EntityFrameworkCore;
namespace InheritanceInEFCore.Data{
    public class FoodItemDbContext:DbContext{
        public DbSet<FoodItem> FoodItems { get; set; }

        public FoodItemDbContext(DbContextOptions<FoodItemDbContext> options): base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FoodItem>()
                .HasDiscriminator<string>("FoodType")
                .HasValue<Pizza>("Pizza")
                .HasValue<Burger>("Burger");
        }
    }
}

