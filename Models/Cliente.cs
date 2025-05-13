using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace libreria.Models;

[Table("Cliente")]
[Index("Email", Name = "UQ__Cliente__A9D10534F6F56046", IsUnique = true)]
public partial class Cliente
{
    [Key]
    public int ClienteId { get; set; }

    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [StringLength(100)]
    public string Apellido { get; set; } = null!;

    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(50)]
    public string? Telefono { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaRegistro { get; set; }

    [InverseProperty("Cliente")]
    public virtual ICollection<Transaccion> Transaccions { get; set; } = new List<Transaccion>();
}
