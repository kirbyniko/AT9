using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using AT9.Models.AbstractTheatre;

namespace AT9.Data
{
    public partial class AbstractTheatreContext : DbContext
    {
        public AbstractTheatreContext()
        {
        }

        public AbstractTheatreContext(DbContextOptions<AbstractTheatreContext> options) : base(options)
        {
        }

        partial void OnModelBuilding(ModelBuilder builder);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AT9.Models.AbstractTheatre.ImageView>()
              .Property(p => p.ViewTime)
              .HasColumnType("datetime");

            builder.Entity<AT9.Models.AbstractTheatre.Order>()
              .Property(p => p.Time)
              .HasColumnType("datetime");

            builder.Entity<AT9.Models.AbstractTheatre.VideoView>()
              .Property(p => p.ViewTime)
              .HasColumnType("datetime");
            this.OnModelBuilding(builder);
        }

        public DbSet<AT9.Models.AbstractTheatre.CartProductUserRelationship> CartProductUserRelationships { get; set; }

        public DbSet<AT9.Models.AbstractTheatre.Category> Categories { get; set; }

        public DbSet<AT9.Models.AbstractTheatre.GalleryImage> GalleryImages { get; set; }

        public DbSet<AT9.Models.AbstractTheatre.GalleryVideo> GalleryVideos { get; set; }

        public DbSet<AT9.Models.AbstractTheatre.AbstractImage> Images { get; set; }

        public DbSet<AT9.Models.AbstractTheatre.ImageView> ImageViews { get; set; }

        public DbSet<AT9.Models.AbstractTheatre.Order> Orders { get; set; }

        public DbSet<AT9.Models.AbstractTheatre.ProductCategoryInstance> ProductCategoryInstances { get; set; }

        public DbSet<AT9.Models.AbstractTheatre.ProductVariation> ProductVariations { get; set; }

        public DbSet<AT9.Models.AbstractTheatre.Product> Products { get; set; }

        public DbSet<AT9.Models.AbstractTheatre.AbstractProfile> Profiles { get; set; }

        public DbSet<AT9.Models.AbstractTheatre.VideoView> VideoViews { get; set; }

        public DbSet<AT9.Models.AbstractTheatre.DeliveryAddress> DeliveryAddresses { get; set; }

        public DbSet<AT9.Models.AbstractTheatre.OrderProductInstance> OrderProductInstances { get; set; }

        public DbSet<AT9.Models.AbstractTheatre.PaymentMethod> PaymentMethods { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        }
    }
}