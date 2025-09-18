using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Identity; // added for password hasher
using System.IO;

var builder = WebApplication.CreateBuilder(args);
var dbPath = Path.Combine(builder.Environment.ContentRootPath, "insecure.db");
var connString = $"Data Source={dbPath}";

builder.Services.AddSingleton(new SqliteConnection(connString));
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>(); // add hasher
builder.Services.AddSingleton<UserRepository>();

// Register sanitation filter
builder.Services.AddScoped<SanitizeInputFilter>();

// Enable MVC Controllers + Razor Pages with global sanitize filter
builder.Services.AddControllersWithViews(o =>
{
    o.Filters.Add<SanitizeInputFilter>();
});
builder.Services.AddRazorPages();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // INSECURE: exposes stack traces
}

app.UseStaticFiles();

// Map Razor Pages (Index, Demo, About)
app.MapRazorPages();

// Map MVC Controllers (Account, Profile, Files)
app.MapDefaultControllerRoute();

// Redirect root (/) to Index
app.MapGet("/", () => Results.Redirect("/Index"));

app.Run();
