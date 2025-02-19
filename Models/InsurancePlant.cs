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
    public string Destination { get; set; }

    public InsuranceType Type { get; set; }

    public InsuranceStatus Status { get; set; }

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeleteAt { get; set; }

    // Chỉ lưu ID của User
    public int? CreatedBy { get; set; }
    public int? UpdatedBy { get; set; }
    public int? DeleteBy { get; set; }

    // Liên kết với User
    [ForeignKey("CreatedBy")]
    public virtual User? Creator { get; set; }
    [ForeignKey("UpdatedBy")]
    public virtual User? Updater { get; set; }
    [ForeignKey("DeleteBy")]
    public virtual User? Deleter { get; set; }


    // Relationships
    public virtual ICollection<InsuranceContract> InsuranceContracts { get; set; }
    public virtual ICollection<InsuranceVehicleDetail> VehicleDetails { get; set; }
    public virtual ICollection<InsuranceLifeDetail> LifeDetails { get; set; }
    public virtual ICollection<InsurancePropertyDetail> PropertyDetails { get; set; }
    public virtual ICollection<InsuranceHealthDetail> HealthDetails { get; set; }
}

public enum InsuranceType
{
    Life,
    Health,
    Auto,
    Property
}

public enum InsuranceStatus
{
    Active,
    Inactive
}
