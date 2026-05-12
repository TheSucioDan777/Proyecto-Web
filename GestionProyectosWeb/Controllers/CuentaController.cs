using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using GestionProyectosWeb.Models;
using Microsoft.Extensions.Configuration;

namespace GestionProyectosWeb.Controllers
{
    public class CuentaController : Controller
    {
        private readonly MiDbContext _context;
        private readonly IConfiguration _config;

        public CuentaController(MiDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // GET: Dibuja la pantalla de Login
        public IActionResult Login()
        {
            // Si el usuario ya inició sesión, lo mandamos directo a sus proyectos
            if (User.Identity!.IsAuthenticated) return RedirectToAction("Index", "Proyectos");
            return View();
        }

        // POST: Procesa el formulario de Login
        [HttpPost]
        public async Task<IActionResult> Login(string Correo, string Password)
        {
            // Buscamos al usuario en la base de datos (incluyendo su Rol)
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Correo == Correo && u.Passwordhash == Password);

            if (usuario != null)
            {
                // 1. Creamos sus "Claims" (Datos de la sesión)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()), // Guardamos su ID
                    new Claim(ClaimTypes.Name, usuario.Nombre),                  // Guardamos su Nombre
                    new Claim(ClaimTypes.Email, usuario.Correo),                 // Guardamos su Correo
                    new Claim(ClaimTypes.Role, usuario.Rol?.Nombre ?? "User")    // Guardamos su Rol
                };

                // 2. Creamos su identidad y credencial
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // 3. Iniciamos la sesión en el navegador (Se crea la Cookie)
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme, 
                    new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Proyectos");
            }

            // Si el correo o contraseña están mal, mostramos un error
            ViewBag.Error = "Correo o contraseña incorrectos";
            return View();
        }

        // GET: Cerrar sesión
        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    
    // GET: Mostrar formulario de Registro
        public IActionResult Registro()
        {
            if (User.Identity!.IsAuthenticated) return RedirectToAction("Index", "Proyectos");
            return View();
        }

        // POST: Procesar el Registro
        [HttpPost]
        public async Task<IActionResult> Registro(string Nombre, string Correo, string Password, string CodigoOrg)
        {
            // 1. Validar el Código de Organización
            var codigoCorrecto = _config["CodigoOrganizacion"];
            if (CodigoOrg != codigoCorrecto)
            {
                ViewBag.Error = "El código de organización es inválido.";
                return View();
            }

            // 2. Validar que el correo no exista ya en la base de datos
            var existeUsuario = await _context.Usuarios.AnyAsync(u => u.Correo == Correo);
            if (existeUsuario)
            {
                ViewBag.Error = "Este correo ya está registrado en el sistema.";
                return View();
            }

            // 3. Crear el nuevo usuario
            var nuevoUsuario = new Usuario
            {
                Nombre = Nombre,
                Correo = Correo,
                Passwordhash = Password, // Ojo: En un entorno 100% real de producción, esto iría encriptado (Hash)
                Rolid = 3 // Le asignamos el Rolid = 3 (Developer) por defecto para que no tenga permisos de Admin
            };

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            // 4. Mandarlo a Iniciar Sesión con un mensaje de éxito
            TempData["MensajeExito"] = "¡Registro exitoso! Ya puedes iniciar sesión.";
            return RedirectToAction("Login");
        }
    
    
    }
}