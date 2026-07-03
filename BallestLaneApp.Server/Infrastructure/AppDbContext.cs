using BallestLaneApp.Server.Domain.Entities;
using BallestLaneApp.Server.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BallestLaneApp.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();

    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureUsers(modelBuilder);
        ConfigureTasks(modelBuilder);

        SeedData(modelBuilder);
    }

    private static void ConfigureUsers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("Users");

            entity.HasKey(user => user.Id);

            entity.Property(user => user.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(user => user.LastName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(user => user.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasIndex(user => user.Email)
                .IsUnique();

            entity.Property(user => user.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(user => user.IsActive)
                .IsRequired();

            entity.Property(user => user.CreatedAt)
                .IsRequired();

            entity.HasMany(user => user.Tasks)
                .WithOne(task => task.User)
                .HasForeignKey(task => task.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureTasks(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("Tasks");

            entity.HasKey(task => task.Id);

            entity.Property(task => task.Title)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(task => task.Description)
                .IsRequired()
                .HasMaxLength(1000);

            entity.Property(task => task.Status)
                .IsRequired()
                .HasConversion<int>();

            entity.Property(task => task.DueDate)
                .IsRequired();

            entity.Property(task => task.UserId)
                .IsRequired();

            entity.Property(task => task.CreatedAt)
                .IsRequired();

            entity.Property(task => task.UpdatedAt)
                .IsRequired(false);

            entity.HasIndex(task => task.UserId);

            entity.HasIndex(task => task.Status);

            entity.HasIndex(task => task.DueDate);
        });
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var demoUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        var demoUser = new ApplicationUser(
            firstName: "Demo",
            lastName: "User",
            email: "demo@ballestlane.com",
            passwordHash: string.Empty);

        modelBuilder.Entity<ApplicationUser>().HasData(new
        {
            Id = demoUserId,
            FirstName = "Demo",
            LastName = "User",
            Email = "demo@ballestlane.com",
            PasswordHash = "AQAAAAIAAYagAAAAEOxaroufEtZBILXxlUkfkkoSU+9fEg6MOjty1yx2zzwTb5XrZ13QMXh2jgy1Eh9Img==",
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        modelBuilder.Entity<TaskItem>().HasData(
            new
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Title = "Prepare technical interview exercise",
                Description = "Create the initial Clean Architecture structure and define entities.",
                Status = TaskItemStatus.InProgress,
                DueDate = new DateTime(2026, 7, 10, 0, 0, 0, DateTimeKind.Utc),
                UserId = demoUserId,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = (DateTime?)null
            },
            new
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Title = "Add unit tests",
                Description = "Cover business rules, validation, repositories, and API endpoints.",
                Status = TaskItemStatus.Pending,
                DueDate = new DateTime(2026, 7, 15, 0, 0, 0, DateTimeKind.Utc),
                UserId = demoUserId,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = (DateTime?)null
            },
            new
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Title = "Create Angular frontend",
                Description = "Build login, register, task list, task form, filters, and user-friendly error messages.",
                Status = TaskItemStatus.Completed,
                DueDate = new DateTime(2026, 7, 20, 0, 0, 0, DateTimeKind.Utc),
                UserId = demoUserId,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}