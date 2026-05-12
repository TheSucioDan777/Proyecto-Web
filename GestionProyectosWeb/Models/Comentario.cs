using System;
using System.Collections.Generic;

namespace GestionProyectosWeb.Models;

public partial class Comentario
{
    public int Id { get; set; }

    public int? Tareaid { get; set; }

    public int? Usuarioid { get; set; }

    public string Contenido { get; set; } = null!;

    public DateTime? Fecha { get; set; }

    public virtual Tarea? Tarea { get; set; }

    public virtual Usuario? Usuario { get; set; }
}
