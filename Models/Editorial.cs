using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace libreria.Models;

[Table("Editorial")]
public partial class Editorial
{
    [Key]
    public int EditorialId { get; set; }

    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [StringLength(200)]
    public string? Direccion { get; set; }

    [StringLength(50)]
    public string? Telefono { get; set; }

    [StringLength(100)]
    public string? Email { get; set; }

    [InverseProperty("Editorial")]
    public virtual ICollection<Libro> Libros { get; set; } = new List<Libro>();
}
