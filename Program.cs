using Microsoft.EntityFrameworkCore;
using WebBanGo.Data;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// CONFIGURATION SERVICES
// ============================================

// Thêm DbContext với SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Server=localhost;Database=webbango;User Id=sa;Password=12345678;TrustServerCertificate=True;Connection Timeout=30"
    )
);

// Thêm Controllers và Views (MVC)
builder.Services.AddControllersWithViews();

// Session (dùng cho giỏ hàng)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// ============================================
// MIDDLEWARE PIPELINE
// ============================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

// Routing mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();



