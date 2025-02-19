using System.ComponentModel.DataAnnotations;

namespace Project_Sem3.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string FullName { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; }

    [Required]
    [MaxLength(255)]
    public string Password { get; set; }

    [Required]
    [StringLength(10)]
    public string Phone { get; set; }

    [Required]
    public Gender Gender { get; set; }

    [Required]
    [StringLength(12)]
    public string CitizenIdentificationCard { get; set; }

    [Required]
    public DateTime DateOfBirth { get; set; }

    public Status Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeleteAt { get; set; }
    [Required]
    public int RoleId { get; set; }

    // Navigation properties
    public Role Role { get; set; }

    // Relationships
    public virtual ICollection<Role> Roles { get; set; }
    public virtual ICollection<InsuranceContract> InsuranceContracts { get; set; }
    public virtual ICollection<InsurancePlan> CreatedInsurancePlans { get; set; }
    public virtual ICollection<InsurancePlan> UpdatedInsurancePlans { get; set; }
    public virtual ICollection<InsurancePlan> DeletedInsurancePlans { get; set; }
    public virtual ICollection<Payment> Payments { get; set; }
    public virtual ICollection<Notification> Notifications { get; set; }
    public virtual ICollection<BorrowCapital> BorrowCapitals { get; set; }
}

public enum Gender
{
    Male,
    Female
}

public enum Status
{
    Deleted = -1,
    Inactive = 0,
    Active = 1
}
