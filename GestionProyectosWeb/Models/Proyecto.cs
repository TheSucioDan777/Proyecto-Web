using System;
using System.Collections.Generic;

namespace GestionProyectosWeb.Models;

public partial class Proyecto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public DateTime? Fechacreacion { get; set; }

    public int? Creadorid { get; set; }

    public virtual Usuario? Creador { get; set; }

    public virtual ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();
}
