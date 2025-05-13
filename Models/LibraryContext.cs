using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace libreria.Models;

public partial class LibraryContext : DbContext
{
    public LibraryContext()
    {
    }

    public LibraryContext(DbContextOptions<LibraryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Editorial> Editorials { get; set; }

    public virtual DbSet<Libro> Libros { get; set; }

    public virtual DbSet<Transaccion> Transaccions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-83R9J92\\MSSQLSERVER01;Database=library;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.ClienteId).HasName("PK__Cliente__71ABD0871787ACBC");

            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<Editorial>(entity =>
        {
            entity.HasKey(e => e.EditorialId).HasName("PK__Editoria__D54C82EE96A8715D");
        });

        modelBuilder.Entity<Libro>(entity =>
        {
            entity.HasKey(e => e.LibroId).HasName("PK__Libro__35A1ECED89BAA6DE");

            entity.HasOne(d => d.Editorial).WithMany(p => p.Libros)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Libro_Editorial");
        });

        modelBuilder.Entity<Transaccion>(entity =>
        {
            entity.HasKey(e => e.TransaccionId).HasName("PK__Transacc__86A849FEEB4F54B5");

            entity.Property(e => e.Cantidad).HasDefaultValue(1);
            entity.Property(e => e.FechaCompra).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Cliente).WithMany(p => p.Transaccions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transaccion_Cliente");

            entity.HasOne(d => d.Libro).WithMany(p => p.Transaccions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transaccion_Libro");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
