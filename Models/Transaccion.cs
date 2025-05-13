using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace libreria.Models;

[Table("Transaccion")]
public partial class Transaccion
{
    [Key]
    public int TransaccionId { get; set; }

    public int ClienteId { get; set; }

    public int LibroId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCompra { get; set; }

    public int Cantidad { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Total { get; set; }

    [ForeignKey("ClienteId")]
    [InverseProperty("Transaccions")]
    public virtual Cliente Cliente { get; set; } = null!;

    [ForeignKey("LibroId")]
    [InverseProperty("Transaccions")]
    public virtual Libro Libro { get; set; } = null!;
}
