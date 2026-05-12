using Microsoft.EntityFrameworkCore;
using GestionProyectosWeb.Models;
using Microsoft.AspNetCore.Authentication.Cookies;


var builder = WebApplication.CreateBuilder(args);

// Configurar la conexión a PostgreSQL
builder.Services.AddDbContext<MiDbContext>(opciones =>
    opciones.UseNpgsql(builder.Configuration.GetConnectionString("ConexionPostgres")));

    // Configurar Autenticación por Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opciones =>
    {
        opciones.LoginPath = "/Cuenta/Login"; // Si alguien sin permiso intenta entrar, lo mandamos aquí
        opciones.ExpireTimeSpan = TimeSpan.FromMinutes(30); // La sesión dura 30 minutos
    });

// Add services to the container.
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
app.UseAuthentication(); // Habilitamos el sistema de autenticación

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
