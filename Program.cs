
using Microsoft.EntityFrameworkCore;
using Project_Sem3.Data;
using Project_Sem3.Helper;
using Project_Sem3.Helper.BaseRate;
using Project_Sem3.Helper.RiskFactor;
using Project_Sem3.Models.MailContent;
using Project_Sem3.Models.MyBank;
using Project_Sem3.Services;
using Project_Sem3.Services.SendMail;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MyDbContext>(options => // Program.cs
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.Configure<PaymentSetting>(builder.Configuration.GetSection("PaymentSetting"));
// Add services to the container.
builder.Services.AddControllersWithViews();
// đăng ký sử dụng file
builder.Services.AddScoped<ISendMailService, SendMailService>();
builder.Services.AddScoped<BaseRate>();
builder.Services.AddScoped<RiskFactor>();
builder.Services.AddScoped<OnlinePaymentServices>();

builder.Services.AddScoped<CalculateCoefficient>(); 
builder.Services.AddScoped<CalculateInsuranceServices>();

var app = builder.Build();

// Configure the HTTP request pipelin
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

