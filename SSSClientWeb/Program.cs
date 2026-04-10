using Microsoft.EntityFrameworkCore;
using SSSClientWeb.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// Register the database context
builder.Services.AddDbContext<SssclientContext>(options =>
    options.UseSqlServer(builder.Configuration
        .GetConnectionString("SSSClientConnection")));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.Run();