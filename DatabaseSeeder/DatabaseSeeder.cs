using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Project_Sem3.Data;
using Project_Sem3.Models;

public class DatabaseSeeder
{
    public static void Seed(MyDbContext context)
    {
// Tạo Roles trước
      if (!context.Roles.Any())
      {
          var roles = new List<Role>
          {
              new Role
              {
                  Name = "Admin",
                  CreatedAt = DateTime.Now,
                  UpdatedAt = DateTime.Now,
                  DeleteAt = null
              },
              new Role
              {
                  Name = "User",
                  CreatedAt = DateTime.Now,
                  UpdatedAt = DateTime.Now,
                  DeleteAt = null
              }
          };
          context.Roles.AddRange(roles);
          context.SaveChanges();
          Console.WriteLine("Roles 'Admin' and 'User' have been created.");
      }

      // Tạo Users
      if (!context.Users.Any())
      {
          try
          {
              // Lấy RoleId của Admin và User từ cơ sở dữ liệu
              var adminRole = context.Roles.FirstOrDefault(r => r.Name == "Admin");
              var userRole = context.Roles.FirstOrDefault(r => r.Name == "User");

              if (adminRole == null || userRole == null)
              {
                  Console.WriteLine("Error: Roles 'Admin' or 'User' not found in database.");
                  return;
              }

              // Tạo tài khoản Admin
              string adminEmail = "admin@gmail.com";
              string adminPassword = "Admin@123";

              if (!context.Users.Any(u => u.Email == adminEmail))
              {
                  var adminUser = new User
                  {
                      FullName = "Administrator",
                      Email = adminEmail,
                      Password = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                      Phone = "0901234567",
                      Gender = Gender.Male,
                      CitizenIdentificationCard = "1234567890",
                      DateOfBirth = DateTime.Now.AddYears(-30),
                      Status = Status.Active,
                      CreatedAt = DateTime.Now,
                      UpdatedAt = DateTime.Now,
                      DeleteAt = null,
                      RoleId = adminRole.Id // Sử dụng Id thực tế từ bảng Roles
                  };
                  context.Users.Add(adminUser);
              }

              // Tạo 10 người dùng ngẫu nhiên
              var userFaker = new Faker<User>()
                  .RuleFor(u => u.FullName, f => f.Name.FullName())
                  .RuleFor(u => u.Email, f => f.Internet.Email())
                  .RuleFor(u => u.Password, f => BCrypt.Net.BCrypt.HashPassword("User@123"))
                  .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber("0#########"))
                  .RuleFor(u => u.Gender, f => f.PickRandom<Gender>())
                  .RuleFor(u => u.CitizenIdentificationCard, f => f.Random.ReplaceNumbers("##########"))
                  .RuleFor(u => u.DateOfBirth, f => f.Date.Past(30, DateTime.Now.AddYears(-18)))
                  .RuleFor(u => u.Status, f => Status.Active)
                  .RuleFor(u => u.CreatedAt, f => f.Date.Past(1))
                  .RuleFor(u => u.UpdatedAt, f => f.Date.Past(1))
                  .RuleFor(u => u.DeleteAt, f => f.Random.Bool(0.1f) ? f.Date.Past(1) : null)
                  .RuleFor(u => u.RoleId, f => f.Random.Bool() ? adminRole.Id : userRole.Id); // Ngẫu nhiên giữa Admin và User

              var users = userFaker.Generate(10);
              context.Users.AddRange(users);

              // Lưu tất cả thay đổi một lần
              context.SaveChanges();

              Console.WriteLine("Admin account and 10 random users created successfully!");
              Console.WriteLine($"Admin Email: {adminEmail}");
              Console.WriteLine($"Admin Password (unhashed): {adminPassword}");
          }
          catch (DbUpdateException ex)
          {
              Console.WriteLine("Error saving changes: " + ex.InnerException?.Message);
              throw;
          }
      }
        if (!context.InsurancePlans.Any())
        {
            var insuranceFaker = new Faker<InsurancePlan>()
                .RuleFor(i => i.Name, f => f.Name.FullName())
                .RuleFor(i => i.Description, f => f.Address.City()) // Thành phố ngẫu nhiên
                .RuleFor(i => i.CoverageAmount, f => f.Random.Decimal(5000000, 50000000)) // Số tiền bảo hiểm
                .RuleFor(i => i.Type, f => f.PickRandom<InsuranceType>())
                .RuleFor(i => i.Status, f => f.PickRandom<InsuranceStatus>())
                .RuleFor(i => i.CreatedAt, f => f.Date.Past(2)) // Ngẫu nhiên trong 2 năm qua
                .RuleFor(i => i.UpdatedAt, f => f.Date.Past(1)) // Ngẫu nhiên trong 1 năm qua
                .RuleFor(i => i.DeleteAt, f => f.Random.Bool(0.1f) ? f.Date.Past(1) : null) // 10% bị xóa
        .RuleFor(i => i.CreatedBy, f => f.Random.Int(1,10)) // Người tạo từ 1-10
        .RuleFor(i => i.UpdatedBy, f => f.Random.Int(1,10))
        .RuleFor(i => i.DeleteBy, f => f.Random.Bool(0.1f) ? f.Random.Int(1,10) : null);

            var insurances = insuranceFaker.Generate(20); // Tạo 20 bản ghi
            context.InsurancePlans.AddRange(insurances);
            context.SaveChanges();
        }
        if (!context.InsuranceContracts.Any())
        {
            var insuranceContractFaker = new Faker<InsuranceContract>()
                .RuleFor(d => d.UserId, f => f.Random.Int(1,10)) // Giả sử có 50 người dùng
                .RuleFor(d => d.PlanId, f => f.Random.Int(1,10)) // Giả sử có 4 gói bảo hiểm
                .RuleFor(d=> d.DetailId,f=> f.Random.Int(1,10))
                .RuleFor(d => d.StartDate, f => f.Date.Past(2)) // Ngày bắt đầu trong vòng 2 năm qua
                .RuleFor(d => d.EndDate, (f, d) => d.StartDate.AddYears(f.Random.Int(1, 10))) // Kết thúc từ 1 đến 10 năm sau
                .RuleFor(d => d.Status, f => f.PickRandom<ContractStatus>()) // Trạng thái hợp đồng
                .RuleFor(d => d.CreatedAt, f => f.Date.Past(2)) // Ngày tạo
                .RuleFor(d => d.UpdatedAt, f => f.Random.Bool(0.5f) ? f.Date.Recent(30) : null) // 50% có ngày cập nhật
                .RuleFor(d => d.DeleteAt, f => f.Random.Bool(0.1f) ? f.Date.Past(1) : null) // 10% bị xóa
                .RuleFor(i => i.CreatedBy, f => f.Random.Int(1,10)) // Người tạo từ 1-10
                .RuleFor(i => i.UpdatedBy, f => f.Random.Int(1,10))
                .RuleFor(i => i.DeleteBy, f => f.Random.Bool(0.1f) ? f.Random.Int(1,10) : null);

            var insuranceContracts = insuranceContractFaker.Generate(100); // Tạo 100 bản ghi
            context.InsuranceContracts.AddRange(insuranceContracts);
            context.SaveChanges();
        }

        if (!context.Payments.Any())
        {
            var paymentFaker = new Faker<Payment>()
                .RuleFor(p => p.UserId, f =>f.Random.Int(1,10)) // Người dùng ngẫu nhiên
                .RuleFor(p => p.ContractId, f => f.Random.Int(1,99)) // Hợp đồng ngẫu nhiên
                .RuleFor(p => p.Amount, f => f.Finance.Amount(100000, 5000000)) // Số tiền ngẫu nhiên
                .RuleFor(p => p.PaymentDate, f => f.Date.Past(1)) // Ngày thanh toán trong năm qua
                .RuleFor(p => p.Status, f => f.PickRandom<PaymentStatus>()) // Trạng thái thanh toán
                .RuleFor(p => p.ImageUrl, f => f.Image.PicsumUrl()) // Ảnh hóa đơn giả lập
                .RuleFor(p => p.CreatedAt, f => f.Date.Past(2)) // Ngẫu nhiên trong 2 năm qua
                .RuleFor(p => p.UpdatedAt, f => f.Date.Past(1)) // Ngẫu nhiên trong 1 năm qua
                .RuleFor(p => p.DeleteAt, f => f.Random.Bool(0.1f) ? f.Date.Past(1) : null) // 10% bị xóa
                .RuleFor(p => p.CreatedBy, f => f.Random.Int(1,10)) // Người tạo từ 1-10
                .RuleFor(p => p.UpdatedBy, f => f.Random.Int(1,10))
                .RuleFor(p => p.DeleteBy, f => f.Random.Bool(0.1f) ? f.Random.Int(1,10) : null);

            var payments = paymentFaker.Generate(50); // Tạo 50 bản ghi
            context.Payments.AddRange(payments);
            context.SaveChanges();
        }


        if (!context.BorrowCapitals.Any())
        {
            var borrowCapitalFaker = new Faker<BorrowCapital>()
                .RuleFor(b => b.UserId, f => f.Random.Int(1,10)) // ID user từ 1-10
                .RuleFor(b => b.LoanAmount, f => f.Finance.Amount(5000000, 50000000)) // Vay từ 5 triệu - 50 triệu
                .RuleFor(b => b.Currency, "VND") // Luôn là VND
                .RuleFor(b => b.InterestRate, f => f.Random.Decimal(3.5m, 15.0m)) // Lãi suất từ 3.5% - 15%
                .RuleFor(b => b.LoanPurpose, f => f.Lorem.Sentence(3)) // Mục đích vay
                .RuleFor(b => b.LoanDate, f => f.Date.Past(2)) // Trong 2 năm qua
                .RuleFor(b => b.RepaymentAmount, (f, b) => b.LoanAmount + (b.LoanAmount * (b.InterestRate / 100))) // Gốc + lãi
                .RuleFor(b => b.DueDate, (f, b) => b.LoanDate.AddMonths(f.Random.Int(6, 36))) // Kỳ hạn 6-36 tháng
                .RuleFor(b => b.Status, f => f.PickRandom<StatusBorrowCapital>())// Trạng thái
                .RuleFor(b => b.CreatedAt, f => f.Date.Past(2))
                .RuleFor(b => b.UpdatedAt, f => f.Date.Past(1))
                .RuleFor(b => b.DeleteAt, f => f.Random.Bool(0.1f) ? f.Date.Past(1) : null) // 10% bị xóa
                .RuleFor(b => b.CreatedBy, f => f.Random.Int(1,10))
                .RuleFor(b => b.UpdatedBy, f => f.Random.Int(1,10))
                .RuleFor(b => b.DeleteBy, f => f.Random.Bool(0.1f) ? f.Random.Int(1,10) : null);

            var borrowCapitals = borrowCapitalFaker.Generate(50); // Tạo 50 bản ghi
            context.BorrowCapitals.AddRange(borrowCapitals);
            context.SaveChanges();
        }
        if (!context.LoanPayments.Any())
        {
          var loanPaymentFaker = new Faker<LoanPayment>()
            .RuleFor(lp => lp.BorrowId, f => f.Random.Int(1, 10))
            .RuleFor(lp => lp.PaymentAmount, f => f.Random.Decimal(100, 5000))
            .RuleFor(lp => lp.PaymentDate, f => f.Date.Past())
            .RuleFor(lp => lp.PaymentImage, f => f.Internet.Avatar())
            .RuleFor(lp => lp.OverdueDays, f => f.Random.Int(0, 30))
            .RuleFor(lp => lp.PenaltyInterest, f => f.Random.Decimal(0, 100))
            .RuleFor(lp => lp.Status, f => f.Random.Bool())
            .RuleFor(lp => lp.CreatedAt, f => f.Date.Past())
            .RuleFor(lp => lp.CreatedBy, f => f.Random.Int(1, 5));
          var loanPayments = loanPaymentFaker.Generate(10);
          context.LoanPayments.AddRange(loanPayments);
        }
        if (!context.Notifications.Any())
        {
            var notificationFaker = new Faker<Notification>()
                .RuleFor(n => n.UserId, f => f.Random.Int(1,10)) // UserId ngẫu nhiên từ 1-10
                .RuleFor(n => n.Message, f => f.Lorem.Sentence(10)) // Nội dung thông báo
                .RuleFor(n => n.IsRead, f => f.Random.Bool(0.3f)) // 30% đã đọc, 70% chưa đọc
                .RuleFor(n => n.CreatedAt, f => f.Date.Past(1)) // Tạo trong vòng 1 năm qua
                .RuleFor(n => n.UpdatedAt, (f, n) => n.IsRead ? f.Date.Recent(30) : null) // Nếu đã đọc thì cập nhật trong 30 ngày qua
                .RuleFor(n => n.DeleteAt, f => f.Random.Bool(0.1f) ? f.Date.Past(1) : null) // 10% bị xóa
                .RuleFor(n => n.CreatedBy, f => f.Random.Int(1,10))
                .RuleFor(n => n.UpdatedBy, f => f.Random.Bool() ? f.Random.Int(1,10): null)
                .RuleFor(n => n.DeleteBy, f => f.Random.Bool(0.1f) ? f.Random.Int(1,10) : null);

            var notifications = notificationFaker.Generate(100); // Tạo 100 thông báo
            context.Notifications.AddRange(notifications);
            context.SaveChanges();
        }

        if (!context.InsuranceLifeDetails.Any())
        {
            var insuranceDetailFaker = new Faker<InsuranceLifeDetail>()
                .RuleFor(d => d.PlanId, f => f.Random.Int(1,10)) // Giả sử có 4 gói bảo hiểm
                .RuleFor(d => d.AnnualPaymentAmount, f => f.Random.Decimal(500000, 5000000)) // Số tiền thanh toán hàng năm
                .RuleFor(d => d.Premium, f => f.Random.Decimal(100000, 1000000)) // Phí bảo hiểm
                .RuleFor(d => d.Deductible, f => f.Random.Decimal(500000, 5000000)) // Khoản khấu trừ
                .RuleFor(d => d.TermYears, f => f.Random.Int(1, 30)) // Thời hạn bảo hiểm
                .RuleFor(d => d.AgeGroup, f => f.PickRandom(new[] { "18-25", "26-35", "36-45", "46-60", "60+" })) // Nhóm tuổi
                .RuleFor(d => d.Beneficiaries, f => f.Name.FullName()) // Người thụ hưởng
                .RuleFor(d => d.Duration, f => f.Random.Int(1, 20)) // Thời gian bảo hiểm (năm)
                .RuleFor(d => d.RiskFactor, f => f.Random.Decimal(0.5m, 2.0m)) // Hệ số rủi ro
                .RuleFor(d => d.Region, f => f.Address.City()) // Khu vực
                .RuleFor(d => d.CreatedAt, f => f.Date.Past(1)) // Ngày tạo trong vòng 1 năm qua
                .RuleFor(d => d.UpdatedAt, f => f.Random.Bool(0.5f) ? f.Date.Recent(30) : null) // 50% có ngày cập nhật
                .RuleFor(d => d.DeleteAt, f => f.Random.Bool(0.1f) ? f.Date.Past(1) : null) // 10% bị xóa
                .RuleFor(n => n.CreatedBy, f => f.Random.Int(1,10))
                .RuleFor(n => n.UpdatedBy, f => f.Random.Bool() ? f.Random.Int(1,10): null)
                .RuleFor(n => n.DeleteBy, f => f.Random.Bool(0.1f) ? f.Random.Int(1,10) : null);

            var insuranceDetails = insuranceDetailFaker.Generate(100); // Tạo 100 bản ghi
            context.InsuranceLifeDetails.AddRange(insuranceDetails);
            context.SaveChanges();
        }
        if (!context.InsuranceHealthDetails.Any())
        {
            var insuranceHealthFaker = new Faker<InsuranceHealthDetail>()
                .RuleFor(d => d.PlanId, f => f.Random.Int(1,10)) // Giả sử có 4 gói bảo hiểm
                .RuleFor(d => d.AnnualPaymentAmount, f => f.Random.Decimal(500000, 5000000)) // Số tiền thanh toán hàng năm
                .RuleFor(d => d.Premium, f => f.Random.Decimal(100000, 1000000)) // Phí bảo hiểm
                .RuleFor(d => d.Deductible, f => f.Random.Decimal(500000, 5000000)) // Khoản khấu trừ
                .RuleFor(d => d.AgeGroup, f => f.PickRandom(new[] { "0-17", "18-25", "26-35", "36-45", "46-60", "60+" })) // Nhóm tuổi
                .RuleFor(d => d.HospitalNetwork, f => f.Company.CompanyName()) // Mạng lưới bệnh viện
                .RuleFor(d => d.PreExistingConditions, f => f.Random.Bool(0.2f) ? f.Lorem.Sentence() : "Không có") // Bệnh lý có sẵn (20% có bệnh)
                .RuleFor(d => d.Duration, f => f.Random.Int(1, 20)) // Thời gian bảo hiểm (năm)
                .RuleFor(d => d.RiskFactor, f => f.Random.Decimal(0.5m, 2.0m)) // Hệ số rủi ro
                .RuleFor(d => d.Region, f => f.Address.City()) // Khu vực
                .RuleFor(d => d.CreatedAt, f => f.Date.Past(1)) // Ngày tạo trong vòng 1 năm qua
                .RuleFor(d => d.UpdatedAt, f => f.Random.Bool(0.5f) ? f.Date.Recent(30) : null) // 50% có ngày cập nhật
                .RuleFor(d => d.DeleteAt, f => f.Random.Bool(0.1f) ? f.Date.Past(1) : null)
                .RuleFor(n => n.CreatedBy, f => f.Random.Int(1,10))
                .RuleFor(n => n.UpdatedBy, f => f.Random.Bool() ? f.Random.Int(1,10): null)
                .RuleFor(n => n.DeleteBy, f => f.Random.Bool(0.1f) ? f.Random.Int(1,10) : null);
                ; // 10% bị xóa

            var insuranceHealthDetails = insuranceHealthFaker.Generate(100); // Tạo 100 bản ghi
            context.InsuranceHealthDetails.AddRange(insuranceHealthDetails);
            context.SaveChanges();
        }
        if (!context.InsurancePropertyDetails.Any())
        {
            var insurancePropertyFaker = new Faker<InsurancePropertyDetail>()
                .RuleFor(d => d.PlanId, f => f.Random.Int(1,10)) // Giả sử có 4 gói bảo hiểm
                .RuleFor(d => d.AnnualPaymentAmount, f => f.Random.Decimal(500000, 5000000)) // Phí hàng năm
                .RuleFor(d => d.Premium, f => f.Random.Decimal(100000, 1000000)) // Phí bảo hiểm
                .RuleFor(d => d.Deductible, f => f.Random.Decimal(500000, 5000000)) // Khoản khấu trừ
                .RuleFor(d => d.PropertyType, f => f.PickRandom(new[] { "Căn hộ", "Nhà phố", "Biệt thự", "Văn phòng", "Kho hàng" })) // Loại tài sản
                .RuleFor(d => d.Location, f => f.Address.StreetAddress()) // Địa điểm
                .RuleFor(d => d.Duration, f => f.Random.Int(1, 30)) // Thời gian bảo hiểm (năm)
                .RuleFor(d => d.RiskFactor, f => f.Random.Decimal(0.5m, 2.0m)) // Hệ số rủi ro
                .RuleFor(d => d.Region, f => f.Address.City()) // Khu vực
                .RuleFor(d => d.CreatedAt, f => f.Date.Past(1)) // Ngày tạo trong vòng 1 năm qua
                .RuleFor(d => d.UpdatedAt, f => f.Random.Bool(0.5f) ? f.Date.Recent(30) : null) // 50% có ngày cập nhật
                .RuleFor(d => d.DeleteAt, f => f.Random.Bool(0.1f) ? f.Date.Past(1) : null)
                .RuleFor(n => n.CreatedBy, f => f.Random.Int(1,10))
                .RuleFor(n => n.UpdatedBy, f => f.Random.Bool() ? f.Random.Int(1,10): null)
                .RuleFor(n => n.DeleteBy, f => f.Random.Bool(0.1f) ? f.Random.Int(1,10) : null);

            var insurancePropertyDetails = insurancePropertyFaker.Generate(100); // Tạo 100 bản ghi
            context.InsurancePropertyDetails.AddRange(insurancePropertyDetails);
            context.SaveChanges();
        }
        if (!context.InsuranceVehicleDetails.Any())
        {
            var insuranceVehicleFaker = new Faker<InsuranceVehicleDetail>()
                .RuleFor(d => d.PlanId, f => f.Random.Int(1,10)) // Giả sử có 4 gói bảo hiểm
                .RuleFor(d => d.AnnualPaymentAmount, f => f.Random.Decimal(300000, 5000000)) // Phí hàng năm
                .RuleFor(d => d.Premium, f => f.Random.Decimal(50000, 1000000)) // Phí bảo hiểm
                .RuleFor(d => d.Deductible, f => f.Random.Decimal(500000, 5000000)) // Khoản khấu trừ
                .RuleFor(d => d.VehicleType, f => f.PickRandom(new[] { "Xe máy", "Ô tô", "Xe tải", "Xe bus", "Xe điện" })) // Loại xe
                .RuleFor(d => d.VehicleModel, f => f.Vehicle.Model()) // Mẫu xe
                .RuleFor(d => d.VehicleYear, f => f.Random.Int(2000, 2024)) // Năm sản xuất
                .RuleFor(d => d.Duration, f => f.Random.Int(1, 10)) // Thời gian bảo hiểm (năm)
                .RuleFor(d => d.RiskFactor, f => f.Random.Decimal(0.5m, 2.0m)) // Hệ số rủi ro
                .RuleFor(d => d.Region, f => f.Address.City()) // Khu vực
                .RuleFor(d => d.CreatedAt, f => f.Date.Past(1)) // Ngày tạo trong vòng 1 năm qua
                .RuleFor(d => d.UpdatedAt, f => f.Random.Bool(0.5f) ? f.Date.Recent(30) : null) // 50% có ngày cập nhật
                .RuleFor(d => d.DeleteAt, f => f.Random.Bool(0.1f) ? f.Date.Past(1) : null)
                .RuleFor(n => n.CreatedBy, f => f.Random.Int(1,10))
                .RuleFor(n => n.UpdatedBy, f => f.Random.Bool() ? f.Random.Int(1,10): null)
                .RuleFor(n => n.DeleteBy, f => f.Random.Bool(0.1f) ? f.Random.Int(1,10) : null);
                ; // 10% bị xóa

            var insuranceVehicleDetails = insuranceVehicleFaker.Generate(100); // Tạo 100 bản ghi
            context.InsuranceVehicleDetails.AddRange(insuranceVehicleDetails);
            context.SaveChanges();
        }
        Console.WriteLine("✅ Database seeded successfully!");
    }
}
