namespace Cadastre.Data
{
    using Cadastre.Data.Models;
    using Microsoft.EntityFrameworkCore;
    public class CadastreContext : DbContext
    {
        public CadastreContext()
        {
            
        }

        public CadastreContext(DbContextOptions options)
            :base(options)
        {
            
        }

        public DbSet<Citizen> Citizens { get; set; } = null!;
        public DbSet<PropertyCitizen> PropertiesCitizens { get; set; } = null!;
        public DbSet<Property> Properties { get; set; } = null!;
        public DbSet<District> Districts { get; set; } = null!;



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PropertyCitizen>(e =>
            {
                e.HasKey(x => new {x.CitizenId,x.PropertyId});

                e.HasOne(x => x.Citizen)
                .WithMany(x => x.PropertiesCitizens)
                .HasForeignKey(x => x.CitizenId)
                .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Property)
                .WithMany(x => x.PropertiesCitizens)
                .HasForeignKey(x => x.PropertyId);
            });
        }
    }
}
