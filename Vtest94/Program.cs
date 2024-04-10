using Microsoft.EntityFrameworkCore;
using Vtest94.Models; // Ensure models are correctly referenced, especially if using UserManager or Identity
using Vtest94.Data;
using Xabe.FFmpeg;
using Vtest94.Repositories;
using Vtest94.Interfaces;
using Vtest94.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Set the path for FFmpeg executables
FFmpeg.SetExecutablesPath(@"C:\ffmpeg\bin");

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure Entity Framework and Identity
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Assuming you are using ASP.NET Core Identity
builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<AppDbContext>();

// Dependency injection for any custom services or repositories
builder.Services.AddScoped<IUploadVideoRepository, UploadVideoRepository>();
builder.Services.AddScoped<IVideoProcessing, VideoProcessing>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Enforces strict transport security
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // This is crucial for Identity to work correctly
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
