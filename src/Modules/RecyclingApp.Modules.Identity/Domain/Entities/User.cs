using RecyclingApp.SharedKernel.Domain;

namespace RecyclingApp.Modules.Identity.Domain.Entities;

/// <summary>
/// Minimal user entity for Phase 1.
/// Provides FK reference targets for user-related columns across the system.
/// Full auth/role model is deferred to Phase 10.
/// </summary>
public class User : EntityBase
{
    public string Username { get; private set; } = null!;
    public string FullName { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private User() { } // EF constructor

    public static User Create(string username, string fullName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);

        return new User
        {
            Username = username.Trim().ToLowerInvariant(),
            FullName = fullName.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string fullName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);
        FullName = fullName.Trim();
        UpdatedAt = DateTime.UtcNow;
    }
}
