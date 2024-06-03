using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using Vtest94.Models;

namespace Vtest94.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseLazyLoadingProxies();
        //}
        public DbSet<Video> Videos { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<UserPhoto> UserPhotos { get; set; }
        public DbSet<VideoStats> VideoStats { get; set; }
        public DbSet<UserLikes> UserLikes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure the User-Video relationship
            builder.Entity<User>()
                .HasMany(u => u.Videos)
                .WithOne(v => v.User)
                .HasForeignKey(v => v.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); // Change delete behavior to Restrict

            // Configure the User-UserPhoto relationship
            builder.Entity<User>()
                .HasOne(u => u.UserPhoto)
                .WithOne(up => up.User)
                .HasForeignKey<UserPhoto>(up => up.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); // Keep delete behavior as Cascade

            // Configure the Category-Video relationship
            builder.Entity<Category>()
                .HasMany(c => c.Videos) // A category has many videos
                .WithOne(v => v.Category) // A video belongs to one category
                .HasForeignKey(v => v.CategoryId) // Foreign key in the Video table
                .IsRequired() // Ensures that the foreign key cannot be null
                .OnDelete(DeleteBehavior.Restrict); // Change delete behavior to Restrict

            // Configure the Video-VideoStats relationship
            builder.Entity<Video>()
                .HasOne(v => v.VideoStats)
                .WithOne()
                .HasForeignKey<Video>(v => v.VideoStatsId)
                .OnDelete(DeleteBehavior.Cascade); // Ensure cascade delete

            builder.Entity<UserLikes>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).HasMaxLength(450);
            });

        }
    }
}
