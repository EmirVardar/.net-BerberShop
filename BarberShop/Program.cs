using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BarberShop.Data;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// "DefaultConnection" adl� ba�lant� dizesini appsettings.json'dan al
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// DbContext servisini kaydet
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity Servisleri
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Cookie Authentication
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Rolleri ve Admin Kullan�c�s�n� Y�net
if (app.Environment.IsDevelopment())
{
    // Rolleri ve Admin Kullan�c�s�n� sadece geli�tirme ortam�nda olu�tur
    await CreateRolesAndAdminUser(app);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Rolleri ve Admin Kullan�c�s�n� Olu�turma Metodu
async Task CreateRolesAndAdminUser(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        // Rolleri olu�tur
        string[] roles = { "Admin", "User" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
                logger.LogInformation($"'{role}' rol� olu�turuldu.");
            }
        }

        // Admin kullan�c�s�n� olu�tur
        var adminUser = await userManager.FindByEmailAsync("admin@example.com");
        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = "admin@example.com",
                Email = "admin@example.com"
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                logger.LogInformation("Admin kullan�c�s� olu�turuldu ve 'Admin' rol�ne atand�.");
            }
            else
            {
                logger.LogError("Admin kullan�c�s� olu�turulurken bir hata olu�tu: {0}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError($"Roller veya admin kullan�c�s� olu�turulurken bir hata olu�tu: {ex.Message}");
    }
}
