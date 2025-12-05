using invenpro.auth.domain.Seedwork;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace invenpro.auth.domain.AggregatesModel.UserAggregate;

[Table("users")]
public class User : IAggregateRoot
{
    [Key]
    [MaxLength(50)]
    public string Id { get; private set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Email { get; private set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; private set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Name { get; private set; } = string.Empty;

    [Required]
    public UserRole Role { get; private set; }

    [MaxLength(500)]
    public string? Avatar { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    private User()
    {
    }

    public User(string id, string email, string passwordHash, string name, UserRole role, string? avatar = null)
    {
        Id = id;
        Email = email.Trim();
        PasswordHash = passwordHash;
        Name = name.Trim();
        Role = role;
        Avatar = avatar;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAvatar(string? avatar)
    {
        Avatar = avatar;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetPasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
        UpdatedAt = DateTime.UtcNow;
    }
}