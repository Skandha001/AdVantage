using System.Diagnostics.Metrics;
using Microsoft.EntityFrameworkCore;
using Trial3.Models;

namespace Trial3.Data
{
    public class Trail3DBContext : DbContext
    {
        public Trail3DBContext(DbContextOptions<Trail3DBContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Ad> Ads { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed User Data
            modelBuilder.Entity<User>().HasData(
                new User { UserID=1000,Username="Admin User",PhoneNumber="9842502235",Email="adminuser@adadvantage.com",Password="adminpass123"}
            );

            // Seed User Data
            modelBuilder.Entity<Ad>().HasData(
                new Ad { AdID = 101,
                    AdType = "Digital Image on Websites", 
                    Description = "Digital Cards, Flash Images, Banners on websites", 
                    PricePerDayOrOneIssue = 200,
                    PriceUnit = "Per CM",
                },
                new Ad
                {
                    AdID = 102,
                    AdType = "Magazine Images",
                    Description = "Digitized or Client specific Image on Popular Magazines",
                    PricePerDayOrOneIssue = 300,
                    PriceUnit = "Per CM",
                },
                new Ad
                {
                    AdID = 103,
                    AdType = "Hand Bills A4 Size",
                    Description = "Multicolour Hand bills printing and distribution",
                    PricePerDayOrOneIssue = 10,
                    PriceUnit = "Per Copy",
                },
                new Ad
                {
                    AdID = 104,
                    AdType = "Highway Road Displays",
                    Description = "Big size Road Flex Displays for 1 month",
                    PricePerDayOrOneIssue = 200,
                    PriceUnit = "Per square feet",
                }
            );

        }
    }
}
