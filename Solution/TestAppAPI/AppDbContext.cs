using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using TestApp;

namespace TestAppAPI
{
    public class AppDbContext : DbContext, IDesignTimeDbContextFactory<AppDbContext>
    {
        public DbSet<StudyGroup> StudyGroups { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        public AppDbContext() { }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public AppDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.Development.json", optional: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            return new AppDbContext(optionsBuilder.Options);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<StudyGroup>(entity =>
            {
                entity.HasKey(e => e.StudyGroupId);
                entity.Property(e => e.StudyGroupId).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(30);
                entity.Property(e => e.Subject).IsRequired().HasConversion<string>();
                entity.Property(e => e.CreateDate).IsRequired();
                entity.HasIndex(e => e.Subject).IsUnique();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<StudyGroup>()
                .HasMany(s => s.Users)
                .WithMany(u => u.StudyGroups)
                .UsingEntity(j => j.ToTable("StudyGroupUsers"));
        }
    }
}