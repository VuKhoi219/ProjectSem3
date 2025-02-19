using Microsoft.EntityFrameworkCore;
using Project_Sem3.Models;

namespace Project_Sem3.Data;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<InsurancePlan> InsurancePlans { get; set; }
    public DbSet<InsuranceVehicleDetail> InsuranceVehicleDetails { get; set; }
    public DbSet<InsuranceLifeDetail> InsuranceLifeDetails { get; set; }
    public DbSet<InsurancePropertyDetail> InsurancePropertyDetails { get; set; }
    public DbSet<InsuranceHealthDetail> InsuranceHealthDetails { get; set; }    public DbSet<InsuranceContract> InsuranceContracts { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<BorrowCapital> BorrowCapitals { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Roles)
            .WithOne()
            .HasForeignKey(r => r.CreatedBy);

        modelBuilder.Entity<InsuranceContract>()
            .HasOne(c => c.User)
            .WithMany(u => u.InsuranceContracts)
            .HasForeignKey(c => c.UserId);

        modelBuilder.Entity<InsuranceContract>()
            .HasOne(c => c.Plan)
            .WithMany(p => p.InsuranceContracts)
            .HasForeignKey(c => c.PlanId);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.User)
            .WithMany(u => u.Payments)
            .HasForeignKey(p => p.UserId);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Contract)
            .WithMany(c => c.Payments)
            .HasForeignKey(p => p.ContractId);

        modelBuilder.Entity<BorrowCapital>()
            .HasOne(b => b.User)
            .WithMany(u => u.BorrowCapitals)
            .HasForeignKey(b => b.UserId);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId);
// Quan hệ giữa InsurancePlan và InsuranceVehicleDetail
        modelBuilder.Entity<InsuranceVehicleDetail>()
            .HasOne(v => v.Plan)
            .WithMany(p => p.VehicleDetails)  // Tên phải khác nhau để tránh nhầm lẫn
            .HasForeignKey(v => v.PlanId);

// Quan hệ giữa InsurancePlan và InsuranceLifeDetail
        modelBuilder.Entity<InsuranceLifeDetail>()
            .HasOne(l => l.Plan)
            .WithMany(p => p.LifeDetails)  // Tên phải khác nhau
            .HasForeignKey(l => l.PlanId);

// Quan hệ giữa InsurancePlan và InsurancePropertyDetail
        modelBuilder.Entity<InsurancePropertyDetail>()
            .HasOne(p => p.Plan)
            .WithMany(p => p.PropertyDetails)  // Tên phải khác nhau
            .HasForeignKey(p => p.PlanId);

// Quan hệ giữa InsurancePlan và InsuranceHealthDetail
        modelBuilder.Entity<InsuranceHealthDetail>()
            .HasOne(h => h.Plan)
            .WithMany(p => p.HealthDetails)  // Tên phải khác nhau
            .HasForeignKey(h => h.PlanId);
        
                modelBuilder.Entity<InsurancePlan>()
            .HasOne(p => p.Creator)
            .WithMany()
            .HasForeignKey(p => p.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InsurancePlan>()
            .HasOne(p => p.Updater)
            .WithMany()
            .HasForeignKey(p => p.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InsurancePlan>()
            .HasOne(p => p.Deleter)
            .WithMany()
            .HasForeignKey(p => p.DeleteBy)
            .OnDelete(DeleteBehavior.Restrict);

        // Cấu hình các bảng chi tiết liên quan đến InsurancePlan
        modelBuilder.Entity<InsuranceVehicleDetail>()
            .HasOne(v => v.Creator)
            .WithMany()
            .HasForeignKey(v => v.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InsuranceVehicleDetail>()
            .HasOne(v => v.Updater)
            .WithMany()
            .HasForeignKey(v => v.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InsuranceVehicleDetail>()
            .HasOne(v => v.Deleter)
            .WithMany()
            .HasForeignKey(v => v.DeleteBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InsuranceLifeDetail>()
            .HasOne(l => l.Creator)
            .WithMany()
            .HasForeignKey(l => l.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InsuranceLifeDetail>()
            .HasOne(l => l.Updater)
            .WithMany()
            .HasForeignKey(l => l.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InsuranceLifeDetail>()
            .HasOne(l => l.Deleter)
            .WithMany()
            .HasForeignKey(l => l.DeleteBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InsurancePropertyDetail>()
            .HasOne(p => p.Creator)
            .WithMany()
            .HasForeignKey(p => p.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InsurancePropertyDetail>()
            .HasOne(p => p.Updater)
            .WithMany()
            .HasForeignKey(p => p.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InsurancePropertyDetail>()
            .HasOne(p => p.Deleter)
            .WithMany()
            .HasForeignKey(p => p.DeleteBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InsuranceHealthDetail>()
            .HasOne(h => h.Creator)
            .WithMany()
            .HasForeignKey(h => h.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InsuranceHealthDetail>()
            .HasOne(h => h.Updater)
            .WithMany()
            .HasForeignKey(h => h.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InsuranceHealthDetail>()
            .HasOne(h => h.Deleter)
            .WithMany()
            .HasForeignKey(h => h.DeleteBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InsuranceContract>()
            .HasOne(c => c.Creator)
            .WithMany()
            .HasForeignKey(c => c.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InsuranceContract>()
            .HasOne(c => c.Updater)
            .WithMany()
            .HasForeignKey(c => c.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InsuranceContract>()
            .HasOne(c => c.Deleter)
            .WithMany()
            .HasForeignKey(c => c.DeleteBy)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Payment>()
            .HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Creator)
            .WithMany()
            .HasForeignKey(p => p.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Updater)
            .WithMany()
            .HasForeignKey(p => p.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Deleter)
            .WithMany()
            .HasForeignKey(p => p.DeleteBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<BorrowCapital>()
            .HasOne(b => b.Creator)
            .WithMany()
            .HasForeignKey(b => b.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<BorrowCapital>()
            .HasOne(b => b.Updater)
            .WithMany()
            .HasForeignKey(b => b.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<BorrowCapital>()
            .HasOne(b => b.Deleter)
            .WithMany()
            .HasForeignKey(b => b.DeleteBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Creator)
            .WithMany()
            .HasForeignKey(n => n.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Updater)
            .WithMany()
            .HasForeignKey(n => n.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Deleter)
            .WithMany()
            .HasForeignKey(n => n.DeleteBy)
            .OnDelete(DeleteBehavior.Restrict);
        // Cấu hình quan hệ giữa Role và User
        modelBuilder.Entity<Role>()
            .HasMany(r => r.Users)
            .WithOne(u => u.Role)  // Giả sử User có thuộc tính 'Role' để liên kết
            .HasForeignKey(u => u.RoleId)  // Khóa ngoại trong bảng User
            .OnDelete(DeleteBehavior.Restrict);

        // Cấu hình các mối quan hệ Creator, Updater, Deleter trong Role
        modelBuilder.Entity<Role>()
            .HasOne(r => r.Creator)
            .WithMany()
            .HasForeignKey(r => r.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Role>()
            .HasOne(r => r.Updater)
            .WithMany()
            .HasForeignKey(r => r.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Role>()
            .HasOne(r => r.Deleter)
            .WithMany()
            .HasForeignKey(r => r.DeleteBy)
            .OnDelete(DeleteBehavior.Restrict);

    }
}