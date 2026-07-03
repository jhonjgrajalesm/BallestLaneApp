namespace BallestLaneApp.Server.Domain.Entities;

public class ApplicationUser
{
    public Guid Id { get; private set; }

    public string FirstName { get; private set; } = string.Empty;

    public string LastName { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public ICollection<TaskItem> Tasks { get; private set; } = new List<TaskItem>();

    private ApplicationUser()
    {
        // Required by EF Core
    }

    public ApplicationUser(
        string firstName,
        string lastName,
        string email,
        string passwordHash)
    {
        Id = Guid.NewGuid();
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email.ToLowerInvariant().Trim();
        PasswordHash = passwordHash;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public string FullName => $"{FirstName} {LastName}".Trim();

    public void UpdateProfile(string firstName, string lastName)
    {
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
    }

    public void ChangePassword(string passwordHash)
    {
        PasswordHash = passwordHash;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}