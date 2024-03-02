using BLL.Interface;
using BLL.Repositery;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var provider = builder.Services.BuildServiceProvider();
//builder.Services.AddDbContext<ApplicationDbContext>();
var config = provider.GetRequiredService<IConfiguration>();
builder.Services.AddDbContext<HellodocContext>(item => item.UseNpgsql(config.GetConnectionString("dbcs")));
builder.Services.AddScoped<ILogin, Login>();
builder.Services.AddScoped<IPatientRequest, PatientRequest>();
builder.Services.AddScoped<IPatientDashboard, PatientDashboard>();
builder.Services.AddScoped<IAdminDashboard, AdminDashboard>();
builder.Services.AddSession();
var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin}/{action=AdminDashboard}/{id?}");

app.Run();
