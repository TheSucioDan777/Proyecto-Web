using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionProyectosWeb.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Configuration; // <-- LIBRERÍA NECESARIA PARA LEER EL appsettings.json

namespace GestionProyectosWeb.Controllers
{
    [Authorize]
    public class ProyectosController : Controller
    {
        private readonly MiDbContext _context;
        private readonly IConfiguration _config;

        // Inyectamos la base de datos en el controlador
        public ProyectosController(MiDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

       // GET: Proyectos
        public async Task<IActionResult> Index()
        {
            // 1. Obtenemos el ID del usuario que tiene la sesión iniciada
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userId = int.Parse(userIdString!);

            // Leemos el código secreto y lo mandamos a la vista
            ViewBag.CodigoOrganizacion = _config["CodigoOrganizacion"];

            // 2. Filtramos: Proyectos que yo creé (Creadorid) Ó (||) Proyectos que tienen alguna de mis tareas
            var misProyectos = await _context.Proyectos
                .Where(p => p.Creadorid == userId || p.Tareas.Any(t => t.Asignadoa == userId))
                .ToListAsync();
            
            // 3. Enviamos los proyectos a la vista
            return View(misProyectos);
        }

        // GET: Proyectos/Crear
        // Esta acción solo dibuja la pantalla con el formulario vacío
        [Authorize(Roles = "Admin,ProjectManager")]
        public IActionResult Crear()
        {
            return View();
        }

        // POST: Proyectos/Crear
        // Esta acción recibe lo que el usuario escribió al darle al botón "Guardar"
        // POST: Proyectos/Crear
        [HttpPost]
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> Crear(string Nombre, string Descripcion)
        {
            // 1. Extraemos el ID del usuario actual leyendo su sesión
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // 2. Lo convertimos a número entero (ya que en PostgreSQL es INT)
            int creadorId = int.Parse(userIdString!);

            var nuevoProyecto = new Proyecto
            {
                Nombre = Nombre,
                Descripcion = Descripcion,
                Fechacreacion = DateTime.Now,
                Creadorid = creadorId // <-- ¡Adiós al 1 fijo! Ahora usamos el ID real
            };

            _context.Proyectos.Add(nuevoProyecto);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}