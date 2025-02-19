using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Project_Sem3.Models;

public class Role
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public int? CreatedBy { get; set; }
    public int? UpdatedBy { get; set; }
    public int? DeleteBy { get; set; }
    public virtual User Creator { get; set; }
    public virtual User Updater { get; set; }
    public virtual User Deleter { get; set; }

    // Relationship
    public virtual ICollection<User> Users { get; set; }
}