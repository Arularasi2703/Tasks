using FoodOrderingSystemAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AccountAPI.Data
{
    public class FoodAppDbContext : DbContext
    {
        public FoodAppDbContext(DbContextOptions<FoodAppDbContext> options) : base(options)
        {
        }

        public DbSet<SignupLogin> SignupLogin { get; set; }
        public DbSet<UserProfile> UserProfile{get;set;}
        // public DbSet<FoodItem> FoodItems { get; set; }
        public DbSet<FoodItem> FoodItems {get;set;}

        public DbSet<FoodCategory> FoodCategories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<InvoiceModel> invoiceModel { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<Checkout> CheckoutDetails { get; set; }
        public DbSet<OtpModel> otpModels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Order>()
                .HasOne(order => order.foodItems)
                .WithMany()
                .HasForeignKey(order => order.foodItemId);

            modelBuilder.Entity<Order>()
                .HasOne(order => order.invoices)
                .WithMany()
                .HasForeignKey(order => order.invoiceId);

            modelBuilder.Entity<InvoiceModel>()
                .HasOne(invoiceModel => invoiceModel.user)
                .WithMany()
                .HasForeignKey(invoiceModel => invoiceModel.userId);
            modelBuilder.Entity<UserProfile>()
                .HasOne(userProfile => userProfile.user)
                .WithMany()
                .HasForeignKey(userProfile => userProfile.userId);

            modelBuilder.Entity<FoodCategory>()
                .HasKey(foodCategory => foodCategory.name);

            modelBuilder.Entity<OtpModel>()
                .HasOne<SignupLogin>(signUp => signUp.user)
                .WithMany()
                .HasForeignKey(user => user.userId) // Use the correct property name: FKUserId
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
