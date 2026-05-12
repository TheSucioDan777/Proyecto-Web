using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GestionProyectosWeb.Models;

public partial class MiDbContext : DbContext
{
    public MiDbContext()
    {
    }

    public MiDbContext(DbContextOptions<MiDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Comentario> Comentarios { get; set; }

    public virtual DbSet<Proyecto> Proyectos { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Tarea> Tareas { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

   // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
  //      => optionsBuilder.UseNpgsql("Host=localhost;Database=GestionProyectosDB;Username=postgres;Password=admin");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comentario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("comentarios_pkey");

            entity.ToTable("comentarios");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Contenido).HasColumnName("contenido");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha");
            entity.Property(e => e.Tareaid).HasColumnName("tareaid");
            entity.Property(e => e.Usuarioid).HasColumnName("usuarioid");

            entity.HasOne(d => d.Tarea).WithMany(p => p.Comentarios)
                .HasForeignKey(d => d.Tareaid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("comentarios_tareaid_fkey");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Comentarios)
                .HasForeignKey(d => d.Usuarioid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("comentarios_usuarioid_fkey");
        });

        modelBuilder.Entity<Proyecto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("proyectos_pkey");

            entity.ToTable("proyectos");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Creadorid).HasColumnName("creadorid");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Fechacreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fechacreacion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .HasColumnName("nombre");

            entity.HasOne(d => d.Creador).WithMany(p => p.Proyectos)
                .HasForeignKey(d => d.Creadorid)
                .HasConstraintName("proyectos_creadorid_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.HasIndex(e => e.Nombre, "roles_nombre_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Tarea>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tareas_pkey");

            entity.ToTable("tareas");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Asignadoa).HasColumnName("asignadoa");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .HasDefaultValueSql("'Pendiente'::character varying")
                .HasColumnName("estado");
            entity.Property(e => e.Fechacreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fechacreacion");
            entity.Property(e => e.Fechavencimiento).HasColumnName("fechavencimiento");
            entity.Property(e => e.Proyectoid).HasColumnName("proyectoid");
            entity.Property(e => e.Titulo)
                .HasMaxLength(150)
                .HasColumnName("titulo");

            entity.HasOne(d => d.AsignadoaNavigation).WithMany(p => p.Tareas)
                .HasForeignKey(d => d.Asignadoa)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("tareas_asignadoa_fkey");

            entity.HasOne(d => d.Proyecto).WithMany(p => p.Tareas)
                .HasForeignKey(d => d.Proyectoid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("tareas_proyectoid_fkey");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("usuarios_pkey");

            entity.ToTable("usuarios");

            entity.HasIndex(e => e.Correo, "usuarios_correo_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .HasColumnName("correo");
            entity.Property(e => e.Fecharegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Passwordhash)
                .HasMaxLength(255)
                .HasColumnName("passwordhash");
            entity.Property(e => e.Rolid).HasColumnName("rolid");

            entity.HasOne(d => d.Rol).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.Rolid)
                .HasConstraintName("usuarios_rolid_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
