using System;
using TechCart.SharedKernel.Entities;
using TechCart.Users.Domain;

namespace TechCart.Users.Domain.Entities;
public class User : AuditableEntity
{
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public UserRole Role { get; private set; }

    private User() { } 

    private User(string firstName, string lastName, string email, string passwordHash, UserRole role)
    {
        Id = Guid.NewGuid();
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        CreatedAt = DateTime.UtcNow;
    }

    // Herkese açık kayıt akışı — rol parametre olarak alınmaz
    public static User Register(string firstName, string lastName, string email, string passwordHash)
        => new(firstName, lastName, email, passwordHash, UserRole.Customer);

    // Sadece admin provisioning/seed için
    public static User CreateAdmin(string firstName, string lastName, string email, string passwordHash)
        => new(firstName, lastName, email, passwordHash, UserRole.Admin);
    
    public void UpdateProfile(string firstName, string lastName, string email)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        MarkUpdated(); // AuditableEntity'den geliyor, UpdatedAt'i now'a çeker
    }
    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        MarkUpdated();
    }

}