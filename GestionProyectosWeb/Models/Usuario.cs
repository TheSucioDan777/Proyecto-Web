using System;
using System.Collections.Generic;

namespace GestionProyectosWeb.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string Passwordhash { get; set; } = null!;

    public int? Rolid { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public virtual ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();

    public virtual ICollection<Proyecto> Proyectos { get; set; } = new List<Proyecto>();

    public virtual Role? Rol { get; set; }

    public virtual ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();
}
