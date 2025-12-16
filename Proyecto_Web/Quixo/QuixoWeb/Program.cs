using Microsoft.EntityFrameworkCore;
using QuixoWeb.Data;
using QuixoWeb.Data.Repositories;
using QuixoWeb.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// GameService en memoria
builder.Services.AddScoped<GameService>();


// EF Core (SQLite por lo que vi)
builder.Services.AddDbContext<QuixoDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Repositorio
builder.Services.AddScoped<IQuixoRepository, QuixoRepository>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
