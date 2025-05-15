using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace libreria.Models;

[Table("Libro")]
[Index("Isbn", Name = "UQ__Libro__447D36EAB869C59B", IsUnique = true)]
public partial class Libro
{
    [Key]
    public int LibroId { get; set; }

    [StringLength(200)]
    public string Titulo { get; set; } = null!;

    [Column("ISBN")]
    [StringLength(20)]
    public string? Isbn { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Precio { get; set; }

    public DateOnly? FechaPublicacion { get; set; }

    public int EditorialId { get; set; }

    [ForeignKey("EditorialId")]
    [InverseProperty("Libros")]
    public virtual Editorial? Editorial { get; set; }

    [InverseProperty("Libro")]
    public virtual ICollection<Transaccion> Transaccions { get; set; } = new List<Transaccion>();
}
