using MyWebApp.Components;
using MyWebApp.Data;
using MyWebApp.Services;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký các dịch vụ cho ứng dụng.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<AuthService>();

// Dữ liệu của đơn vị dùng chung cho các phiên và mất đi khi ứng dụng dừng.
builder.Services.AddSingleton<MockDataService>();
builder.Services.AddScoped<VehicleService>();
builder.Services.AddScoped<DriverService>();
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<DispatchService>();
builder.Services.AddScoped<TripService>();
builder.Services.AddScoped<OfficerService>();
builder.Services.AddScoped<MaintenanceService>();
builder.Services.AddScoped<AppNotificationService>();

builder.Services.AddRadzenComponents();

var app = builder.Build();

// Cấu hình các bước xử lý yêu cầu từ trình duyệt.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
