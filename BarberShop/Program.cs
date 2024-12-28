using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BarberShop.Data;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// "DefaultConnection" adlý baðlantý dizesini appsettings.json'dan al
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// DbContext servisini kaydet
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity Servisleri
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // Þifre doðrulama kurallarýný devre dýþý býrak
    options.Password.RequireDigit = false;            // Rakam gereksinimi kaldýrýlýr
    options.Password.RequiredLength = 1;              // Minimum þifre uzunluðu 1 karakter
    options.Password.RequireNonAlphanumeric = false;  // Özel karakter gereksinimi kaldýrýlýr
    options.Password.RequireUppercase = false;        // Büyük harf gereksinimi kaldýrýlýr
    options.Password.RequireLowercase = false;        // Küçük harf gereksinimi kaldýrýlýr
    options.Password.RequiredUniqueChars = 0;         // Benzersiz karakter gereksinimi kaldýrýlýr
})
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

// Rolleri ve Admin Kullanýcýsýný Yönet
if (app.Environment.IsDevelopment())
{
    // Rolleri ve Admin Kullanýcýsýný sadece geliþtirme ortamýnda oluþtur
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

// Rolleri ve Admin Kullanýcýsýný Oluþturma Metodu
async Task CreateRolesAndAdminUser(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        // Rolleri oluþtur
        string[] roles = { "Admin", "User" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
                logger.LogInformation($"'{role}' rolü oluþturuldu.");
            }
        }

        // Admin kullanýcýsýný oluþtur
        var adminUser = await userManager.FindByEmailAsync("g211210071@sakarya.edu.tr");
        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = "g211210071@sakarya.edu.tr",
                Email = "g211210071@sakarya.edu.tr"
            };

            var result = await userManager.CreateAsync(adminUser, "sau");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                logger.LogInformation("Admin kullanýcýsý oluþturuldu ve 'Admin' rolüne atandý.");
            }
            else
            {
                logger.LogError("Admin kullanýcýsý oluþturulurken bir hata oluþtu: {0}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError($"Roller veya admin kullanýcýsý oluþturulurken bir hata oluþtu: {ex.Message}");
    }
}
