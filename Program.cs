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
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // INSECURE: detailed errors
}

app.UseStaticFiles(); // exposes wwwroot/uploads

app.MapDefaultControllerRoute();
app.Run();
