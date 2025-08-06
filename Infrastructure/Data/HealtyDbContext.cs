using Infrastructure.Entities;
using Infrastructure.Entities.Shared;
using Infrastructure.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class HealtyDbContext : DbContext
{
    public HealtyDbContext(DbContextOptions<HealtyDbContext> options) : base(options)
    {
        Database.SetCommandTimeout(200);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Diaries> Diaries { get; set; }
    public DbSet<Exercise> Exercise { get; set; }
    public DbSet<Food> Food { get; set; }
    public DbSet<Meal> Meal { get; set; }
    public DbSet<BodyRecords> BodyRecord { get; set; }
    public DbSet<Recommend> Recommends { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

        /// Table : Users
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.FirstName)
                  .HasMaxLength(64)
                  .IsRequired();

            entity.Property(u => u.LastName)
                  .HasMaxLength(64)
                  .IsRequired();

            entity.Property(u => u.MailAddress)
                  .HasMaxLength(256)
                  .IsRequired();

            entity.Property(u => u.LoginPassword)
                  .HasMaxLength(128);

            entity.Property(u => u.Version)
                  .IsRowVersion();

            entity.HasMany(u => u.Diaries)
                  .WithOne(d => d.User)
                  .HasForeignKey(d => d.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(u => u.BodyRecords)
                 .WithOne(d => d.User)
                 .HasForeignKey(d => d.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
        });

        /// Table : Diaries
        modelBuilder.Entity<Diaries>(entity =>
        {
            entity.HasKey(d => d.Id);

            entity.Property(d => d.Title)
                  .HasMaxLength(255);

            entity.Property(d => d.Content)
                  .HasMaxLength(500);

            entity.Property(d => d.Version)
                  .IsRowVersion();

            entity.HasOne(d => d.User)
                  .WithMany(u => u.Diaries)
                  .HasForeignKey(d => d.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        /// Table : Meals
        modelBuilder.Entity<Meal>(entity =>
        {
            entity.HasKey(m => m.Id);

            entity.Property(m => m.MealType)
                  .HasConversion<int>()
                  .HasDefaultValue(MealType.Morning)
                  .IsRequired();

            entity.Property(m => m.MealDate)
                .IsRequired();

            entity.Property(m => m.CreatedAt)
                  .HasDefaultValueSql("GETDATE()");

            entity.HasIndex(m => new { m.UserId, m.MealType, m.MealDate })
                  .IsUnique();

            modelBuilder.Entity<Meal>()
                .HasOne(m => m.User)
                .WithMany(u => u.Meals)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Meal>()
               .HasOne(m => m.Food)
               .WithMany(f => f.Meals)
               .HasForeignKey(m => m.FoodId)
               .OnDelete(DeleteBehavior.Restrict);
        });

        /// Table : Foods
        modelBuilder.Entity<Food>(entity =>
        {
            entity.HasKey(f => f.Id);

            entity.Property(f => f.Name)
                  .HasMaxLength(255)
                  .IsRequired();

            entity.Property(f => f.PhotoUrl)
                  .HasMaxLength(512);

            entity.HasMany(f => f.Meals)
                  .WithOne(m => m.Food)
                  .HasForeignKey(m => m.FoodId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        /// Table : Exercises
        modelBuilder.Entity<Exercise>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                  .HasMaxLength(255)
                  .IsRequired();

            entity.Property(e => e.Description)
                  .HasMaxLength(1000);

            entity.Property(e => e.DurationMinutes)
                  .IsRequired();

            entity.Property(e => e.ExerciseDate)
                  .IsRequired();

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("GETDATE()");

            entity.HasOne(e => e.User)
                  .WithMany(u => u.Exercises)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        /// Table : BodyRecords
        modelBuilder.Entity<BodyRecords>()
            .HasOne(w => w.User)
            .WithMany(u => u.BodyRecords)
            .HasForeignKey(w => w.UserId);

        /// Table : Recommend
        modelBuilder.Entity<Recommend>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.ImageUrl)
                  .IsRequired();

            entity.Property(e => e.Content)
                  .HasMaxLength(1000);

            entity.Property(e => e.HashTags)
                  .HasMaxLength(2000);

            entity.Property(e => e.CreatedAt);
            entity.Property(e => e.CreatedUserId);
            entity.Property(e => e.UpdatedAt);
            entity.Property(e => e.UpdatedUserId);
            entity.Property(e => e.DeletedAt);
            entity.Property(e => e.DeletedUserId);
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken  = default)
    {
        var now = DateTime.Now;
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is IHasTimestamps entity)
            {
                switch (entry.State)
                {
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entity.DeletedAt = now;
                        continue;
                    case EntityState.Modified:
                        entry.State = EntityState.Modified;
                        entity.UpdatedAt = now;
                        continue;
                    case EntityState.Added:
                        entry.State = EntityState.Added;
                        entity.CreatedAt = now;
                        continue;
                    case EntityState.Detached:
                    case EntityState.Unchanged:
                    default:
                        continue;
                }
            }
        }

        var result = await base.SaveChangesAsync(cancellationToken);
        base.ChangeTracker.Clear();
        return result;
    }
}
