using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using P2PCryptoScaner.Models;
using P2PCryptoScaner.Services.Initializer;

namespace P2PCryptoScaner.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Level> Levels { get; set; }
        public DbSet<LevelPurchase> LevelPurchases { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            this.Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(user =>
            {
                user.Property(u => u.Id)
                .ValueGeneratedOnAdd();

                user.HasIndex(u => u.Email)
                .IsUnique();
            });

            modelBuilder.Entity<Level>(level =>
            {
                level.Property(l => l.Id)
                .ValueGeneratedOnAdd();

                level.Property(l => l.Duration)
                .HasConversion<long>();
            });

            modelBuilder.Entity<LevelPurchase>(levelPurchase =>
            {
                levelPurchase.Property(lp => lp.Id)
                .ValueGeneratedOnAdd();

                levelPurchase.Property(lp => lp.LevelDuration)
                .HasConversion<long>();

                levelPurchase
                .HasOne(lp => lp.User)
                .WithMany(u => u.PurchaseHistory)
                .HasForeignKey(lp => lp.UserId)
                .HasPrincipalKey(u => u.Id)
                .OnDelete(DeleteBehavior.ClientCascade);

                levelPurchase
                .HasOne(lp => lp.Level)
                .WithMany(l => l.LevelPurchases)
                .HasForeignKey(lp => lp.LevelId)
                .HasPrincipalKey(l => l.Id)
                .OnDelete(DeleteBehavior.ClientCascade);
            });

            


            base.OnModelCreating(modelBuilder);
        }
    }
}