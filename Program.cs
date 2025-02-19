using System.Collections.Immutable;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Project_Sem3.Data;
using Project_Sem3.Helper;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MyDbContext>(options => // Program.cs
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); 


// Add services to the container.
builder.Services.AddControllersWithViews();
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

