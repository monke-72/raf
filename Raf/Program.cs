using Microsoft.EntityFrameworkCore;
using Raf.Data;
using Raf.Services;

var builder = WebApplication.CreateBuilder(args);

// Gerçek bağlantı dizesi user-secrets'tan gelir, appsettings.json'da boş durur.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Bağlantı dizesi bulunamadı. README'deki 'dotnet user-secrets set' adımını uygulayın.");
}

builder.Services.AddDbContext<RafDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddHttpClient<ITmdbService, TmdbService>(client =>
{
    client.BaseAddress = new Uri("https://api.themoviedb.org/3/");
});

builder.Services.AddHttpClient<IRawgService, RawgService>(client =>
{
    client.BaseAddress = new Uri("https://api.rawg.io/api/");
    client.DefaultRequestHeaders.UserAgent.ParseAdd("Raf/1.0");
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

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

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
