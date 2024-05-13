using Microsoft.EntityFrameworkCore;
using Vtest94.Models; // Ensure models are correctly referenced, especially if using UserManager or Identity
using Vtest94.Data;
using Xabe.FFmpeg;
using Vtest94.Repositories;
using Vtest94.Interfaces;
using Vtest94.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

var builder = WebApplication.CreateBuilder(args);

// Set the path for FFmpeg executables
FFmpeg.SetExecutablesPath(@"C:\ffmpeg\bin");

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure Entity Framework and Identity
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Assuming you are using ASP.NET Core Identity
//builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
//                .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddIdentity<User, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    // Configure the default sign-in and sign-out schemes
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Your login path
})
.AddGoogle(options =>
{
    options.ClientId = "203660731378-7pnc70l2vknvgls6h65ngo4p003rr3r7.apps.googleusercontent.com";
    options.ClientSecret = "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";
    options.CallbackPath = new PathString("/signin-google");
    options.SignInScheme = IdentityConstants.ExternalScheme;

    // Requesting additional scopes
    options.Scope.Add("https://www.googleapis.com/auth/userinfo.profile");
    options.Scope.Add("https://www.googleapis.com/auth/userinfo.email");
});

// Dependency injection for any custom services or repositories
builder.Services.AddScoped<IUploadVideoRepository, UploadVideoRepository>();
builder.Services.AddScoped<IVideoProcessing, VideoProcessing>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

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
    pattern: "{controller=Video}/{action=Index}/{id?}");

app.Run();
