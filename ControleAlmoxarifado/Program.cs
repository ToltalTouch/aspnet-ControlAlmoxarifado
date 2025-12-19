using Microsoft.EntityFrameworkCore;
using ControleAlmoxarifado.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Register ApplicationDbContext. Use SQL Server LocalDB (AlmoxarifadoDb) in development.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    // fallback to in-memory if no connection string provided
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseInMemoryDatabase("ControleDB"));
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));
}

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Ensure database/schema exists when using SQL Server (creates tables for the current model if missing).
try
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var db = services.GetRequiredService<ApplicationDbContext>();
        db.Database.EnsureCreated();
        app.Logger.LogInformation("Database ensured/created.");
    }
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "An error occurred while ensuring the database was created.");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

// Serve static files from wwwroot (replacement for missing MapStaticAssets extension)
app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    // make Almoxarifado the default controller so the inventory index is the site root
    pattern: "{controller=Almoxarifado}/{action=Index}/{id?}")
    ;


app.Run();
