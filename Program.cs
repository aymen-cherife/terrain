using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using terrain;
using terrain.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), new MySqlServerVersion(new Version(8, 0, 21))));

// Add session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Use session and authentication
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// User routes
app.MapControllerRoute(
    name: "user_register",
    pattern: "User/Register",
    defaults: new { controller = "UserAuth", action = "Register" });

app.MapControllerRoute(
    name: "user_login",
    pattern: "User/Login",
    defaults: new { controller = "UserAuth", action = "Login" });

app.MapControllerRoute(
    name: "user_reservations",
    pattern: "User/Reservations",
    defaults: new { controller = "Reservations", action = "UserIndex" });

// Manager routes
app.MapControllerRoute(
    name: "manager_login",
    pattern: "Manager/Login",
    defaults: new { controller = "ManagerAuth", action = "Login" });

app.MapControllerRoute(
    name: "manager_reservations",
    pattern: "Manager/Reservations",
    defaults: new { controller = "Reservations", action = "ManagerIndex" });

// Terrain routes
app.MapControllerRoute(
    name: "terrain_crud_table",
    pattern: "Manager/Terrains",
    defaults: new { controller = "Terrains", action = "Index" });

app.Run();
