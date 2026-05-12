using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionProyectosWeb.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims; // <-- LIBRERÍA NECESARIA PARA LEER LA SESIÓN

namespace GestionProyectosWeb.Controllers
{
    [Authorize]
    public class TareasController : Controller
    {
        private readonly MiDbContext _context;

        public TareasController(MiDbContext context)
        {
            _context = context;
        }

        // Esta acción recibe el ID del proyecto que clickeaste
        public async Task<IActionResult> Index(int id)
        {
            var proyecto = await _context.Proyectos.FindAsync(id);
            if (proyecto == null) 
            {
                return NotFound();
            }
            
            ViewBag.NombreProyecto = proyecto.Nombre;
            ViewBag.ProyectoId = id; 

            var tareas = await _context.Tareas
                .Where(t => t.Proyectoid == id)
                .ToListAsync();

            return View(tareas);
        }

        // GET: Tareas/Detalles/5
        public async Task<IActionResult> Detalles(int id)
        {
            var tarea = await _context.Tareas
                .Include(t => t.AsignadoaNavigation) 
                .Include(t => t.Comentarios)         
                    .ThenInclude(c => c.Usuario)     
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tarea == null)
            {
                return NotFound();
            }

            return View(tarea);
        }

        // GET: Tareas/Crear?proyectoId=X
        [Authorize(Roles = "Admin,ProjectManager")]
        public IActionResult Crear(int proyectoId)
        {
            ViewBag.ProyectoId = proyectoId;
            ViewBag.Usuarios = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Usuarios, "Id", "Nombre");
            
            return View();
        }

        // POST: Tareas/Crear
        [HttpPost]
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> Crear(int Proyectoid, string Titulo, string Descripcion, int Asignadoa, DateOnly? FechaVencimiento)
        {
            var nuevaTarea = new Tarea
            {
                Proyectoid = Proyectoid,
                Titulo = Titulo,
                Descripcion = Descripcion,
                Estado = "Pendiente", 
                Asignadoa = Asignadoa,
                Fechavencimiento = FechaVencimiento,
                Fechacreacion = DateTime.Now
            };

            _context.Tareas.Add(nuevaTarea);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { id = Proyectoid });
        }

        // POST: Tareas/CambiarEstado
        [HttpPost]
        public async Task<IActionResult> CambiarEstado(int id, string estado, int proyectoId)
        {
            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea != null)
            {
                tarea.Estado = estado;
                _context.Update(tarea);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", new { id = proyectoId });
        }

        // POST: Tareas/AgregarComentario
        [HttpPost]
        public async Task<IActionResult> AgregarComentario(int TareaId, string Contenido)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int usuarioId = int.Parse(userIdString!);

            var nuevoComentario = new Comentario
            {
                Tareaid = TareaId,
                Usuarioid = usuarioId,
                Contenido = Contenido,
                Fecha = DateTime.Now
            };

            _context.Comentarios.Add(nuevoComentario);
            await _context.SaveChangesAsync();

            return RedirectToAction("Detalles", new { id = TareaId });
        }

    } // <-- AHORA SÍ, LA CLASE SE CIERRA AQUÍ DESPUÉS DE TODOS LOS MÉTODOS
} // <-- Y AQUÍ SE CIERRA EL NAMESPACE