using BookingSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Domain.Entities;

public class ServiceProvider : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string Specialty { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = false;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    //navigation properties
    public ICollection<WorkingHours> WorkingHours { get; private set; } = new List<WorkingHours>();
    public ICollection<Appointment> Appointments { get; private set; } = new List<Appointment>();
    public ICollection<BlockedTime> BlockedTimes { get; private set; } = new List<BlockedTime>();

    // EF core needs emtpy constructor
    private ServiceProvider() { }

    //static factory method 
    public static ServiceProvider Create(string  name, string email, string passwordHash, string specialty)
    {
        return new ServiceProvider()
        {
            Name = name,
            Email = email,
            PasswordHash = passwordHash,
            Specialty = specialty,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };
    }

    //business methods
    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    public void UpdateProfile(string name, string specialty)
    {
        Name = name;
        Specialty = specialty;
    }

    public void UpdatePasswordHasher(string passwordHash) => 
        PasswordHash = passwordHash;

}
