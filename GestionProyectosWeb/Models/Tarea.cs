using System;
using System.Collections.Generic;

namespace GestionProyectosWeb.Models;

public partial class Tarea
{
    public int Id { get; set; }

    public int? Proyectoid { get; set; }

    public string Titulo { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string? Estado { get; set; }

    public int? Asignadoa { get; set; }

    public DateOnly? Fechavencimiento { get; set; }

    public DateTime? Fechacreacion { get; set; }

    public virtual Usuario? AsignadoaNavigation { get; set; }

    public virtual ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();

    public virtual Proyecto? Proyecto { get; set; }
}
