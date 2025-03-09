using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project_Sem3.Data;
using Project_Sem3.Helper;
using Project_Sem3.Helper.BaseRate;
using Project_Sem3.Helper.RiskFactor;
using Project_Sem3.Models;
using Project_Sem3.Models.InterestRate;
using Project_Sem3.Models.MailContent;
using Project_Sem3.Models.MyBank;
using Project_Sem3.Services;
using Project_Sem3.Services.CalculateBorrowCapital;
using Project_Sem3.Services.CalculateBorrowCapital.MonthlyPaymentAmount;
using Project_Sem3.Services.LoanPaymentServices;
using Project_Sem3.Services.LoanPaymentServices.CheckIfOverdue;
using Project_Sem3.Services.LoanPaymentServices.PenaltyPercentage;
using Project_Sem3.Services.SendMail;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MyDbContext>(options => // Program.cs
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.Configure<PaymentSetting>(builder.Configuration.GetSection("PaymentSetting"));
builder.Services.Configure<InterestRateSetting>(builder.Configuration.GetSection("FixeInterestSetting"));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
// Add services to the container.
builder.Services.AddControllersWithViews()
  .AddJsonOptions(options =>
  {
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    options.JsonSerializerOptions.WriteIndented = true;
  });
// đăng ký sử dụng file
builder.Services.AddScoped<MonthlyPaymentAmount>();
builder.Services.AddScoped<CalculateBorrowCapitalServices>();
builder.Services.AddScoped<ISendMailService, SendMailService>();
builder.Services.AddScoped<BaseRate>();
builder.Services.AddScoped<RiskFactor>();
builder.Services.AddScoped<OnlinePaymentServices>();
builder.Services.AddScoped<CalculateCoefficient>();
builder.Services.AddScoped<CalculateInsuranceServices>();
builder.Services.AddScoped<LoanPaymentServices>();
builder.Services.AddScoped<CalculatePenaltyPercentage>();
builder.Services.AddScoped<CalculateCheckIfOverdue>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
  options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian hết hạn session (30 phút)
  options.Cookie.HttpOnly = true;
  options.Cookie.IsEssential = true;
});
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
  .AddEntityFrameworkStores<MyDbContext>()
  .AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options =>
{
  options.LoginPath = "/Auth/Login"; // Account/Login <- default
  options.AccessDeniedPath = "/Auth/AccessDenied"; // Trang 403, đăng nhập rồi nhưng không phải admin hoặc ...
  options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
});

var app = builder.Build();

var scopeAuth = app.Services.CreateScope();
// tạo mới role
var roleManager = scopeAuth.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
// tạo mới user
var userManager = scopeAuth.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

if (!await roleManager.RoleExistsAsync("Admin"))
{
  await roleManager.CreateAsync(new IdentityRole("Admin"));
}

var adminUser = new IdentityUser
{
  UserName = "admin@example19.com",
  Email = "admin@example.com",
  EmailConfirmed = true
};

if (await userManager.FindByEmailAsync(adminUser.Email) == null)
{
  var result = await userManager.CreateAsync(adminUser, "Admin@123");
  if (result.Succeeded)
  {
    await userManager.AddToRoleAsync(adminUser, "Admin");
  }
}

// Configure the HTTP request pipelin
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
if (args.Contains("--seed"))
{
  using (var scope = app.Services.CreateScope())
  {
    var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();
    DatabaseSeeder.Seed(dbContext);
    Console.WriteLine("✅ Dữ liệu InsuranceVehicleDetail đã được seed!");
  }
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
  name: "default",
  pattern: "{area:exists}/{controller=Home}/{action=Index2}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();

