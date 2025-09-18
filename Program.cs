using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Data.Sqlite;
using System.IO;

var builder = WebApplication.CreateBuilder(args);
var dbPath = Path.Combine(builder.Environment.ContentRootPath, "insecure.db");
var connString = $"Data Source={dbPath}";

builder.Services.AddSingleton(new SqliteConnection(connString));
builder.Services.AddSingleton<UserRepository>();

// Enable MVC Controllers + Razor Pages
builder.Services.AddControllersWithViews();
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
