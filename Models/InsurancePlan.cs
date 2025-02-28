using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Sem3.Models;

public class InsurancePlan
{
    [Key]
    public int Id { get; set; }

    [MaxLength(255)]
    public string Name { get; set; }

    [MaxLength(225)]
    public string Description { get; set; }
    [Required]
    public InsuranceType Type { get; set; }
    [Required]
    public InsuranceStatus Status { get; set; }
    [Required]
    public decimal CoverageAmount { get; set; } // Thêm trường này
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeleteAt { get; set; }

    // Chỉ lưu ID của User
    public int? CreatedBy { get; set; }
    public int? UpdatedBy { get; set; }
    public int? DeleteBy { get; set; }

    // Liên kết với User
    public virtual User? Creator { get; set; }
    public virtual User? Updater { get; set; }
    public virtual User? Deleter { get; set; }


    // Relationships
    public virtual ICollection<InsuranceContract>? InsuranceContracts { get; set; }
    public virtual ICollection<InsuranceVehicleDetail>? VehicleDetails { get; set; }
    public virtual ICollection<InsuranceLifeDetail>? LifeDetails { get; set; }
    public virtual ICollection<InsurancePropertyDetail>? PropertyDetails { get; set; }
    public virtual ICollection<InsuranceHealthDetail>? HealthDetails { get; set; }
}

public enum InsuranceType
{
    Life,
    Health,
    Vehicle,
    Property
}

public enum InsuranceStatus
{
    Active,
    Inactive
}
