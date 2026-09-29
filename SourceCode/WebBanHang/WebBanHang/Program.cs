using WebBanHang.service;

var builder = WebApplication.CreateBuilder(args);
//Connect VNPay API
builder.Services.AddScoped<IVnPayService, VnPayService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Đăng ký Session
builder.Services.AddDistributedMemoryCache();


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2); // Hết hạn sau 2 giờ không hoạt động
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

// Kích hoạt Session
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();