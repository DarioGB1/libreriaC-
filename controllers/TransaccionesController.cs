using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using libreria.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace libreria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransaccionesController : ControllerBase
    {
        private readonly LibraryContext _context;

        public TransaccionesController(LibraryContext context)
        {
            _context = context;
        }

        // GET: api/Transacciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaccion>>> GetTransacciones()
        {
            return await _context.Transaccions
                .Include(t => t.Cliente)
                .Include(t => t.Libro)
                .ToListAsync();
        }

        // GET: api/Transacciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Transaccion>> GetTransaccion(int id)
        {
            var transaccion = await _context.Transaccions
                .Include(t => t.Cliente)
                .Include(t => t.Libro)
                .FirstOrDefaultAsync(t => t.TransaccionId == id);

            if (transaccion == null)
            {
                return NotFound();
            }

            return transaccion;
        }

        // GET: api/Transacciones/cliente/5
        [HttpGet("cliente/{clienteId}")]
        public async Task<ActionResult<IEnumerable<Transaccion>>> GetTransaccionesByCliente(int clienteId)
        {
            if (!await _context.Clientes.AnyAsync(c => c.ClienteId == clienteId))
            {
                return NotFound("Cliente no encontrado");
            }

            return await _context.Transaccions
                .Include(t => t.Libro)
                .Where(t => t.ClienteId == clienteId)
                .ToListAsync();
        }

        // GET: api/Transacciones/libro/5
        [HttpGet("libro/{libroId}")]
        public async Task<ActionResult<IEnumerable<Transaccion>>> GetTransaccionesByLibro(int libroId)
        {
            if (!await _context.Libros.AnyAsync(l => l.LibroId == libroId))
            {
                return NotFound("Libro no encontrado");
            }

            return await _context.Transaccions
                .Include(t => t.Cliente)
                .Where(t => t.LibroId == libroId)
                .ToListAsync();
        }

        // Clase DTO para crear transacciones
        public class TransaccionCreateDto
        {
            public int ClienteId { get; set; }
            public int LibroId { get; set; }
            public DateTime? FechaCompra { get; set; }
            public int Cantidad { get; set; }
        }

        // POST: api/Transacciones
        [HttpPost]
        public async Task<ActionResult<Transaccion>> PostTransaccion(TransaccionCreateDto transaccionDto)
        {
            // Check if client exists
            if (!await _context.Clientes.AnyAsync(c => c.ClienteId == transaccionDto.ClienteId))
            {
                return BadRequest("El cliente especificado no existe");
            }

            // Check if book exists
            var libro = await _context.Libros.FindAsync(transaccionDto.LibroId);
            if (libro == null)
            {
                return BadRequest("El libro especificado no existe");
            }

            var transaccion = new Transaccion
            {
                ClienteId = transaccionDto.ClienteId,
                LibroId = transaccionDto.LibroId,
                // Set purchase date to now if not provided
                FechaCompra = transaccionDto.FechaCompra ?? DateTime.Now,
                // Set default quantity if not provided
                Cantidad = transaccionDto.Cantidad <= 0 ? 1 : transaccionDto.Cantidad
            };

            // Calculate total based on book price and quantity
            transaccion.Total = libro.Precio * transaccion.Cantidad;

            _context.Transaccions.Add(transaccion);
            await _context.SaveChangesAsync();

            // Load related entities for the response
            await _context.Entry(transaccion).Reference(t => t.Cliente).LoadAsync();
            await _context.Entry(transaccion).Reference(t => t.Libro).LoadAsync();

            return CreatedAtAction("GetTransaccion", new { id = transaccion.TransaccionId }, transaccion);
        }

        // Clase DTO para actualizar transacciones
        public class TransaccionUpdateDto
        {
            public int TransaccionId { get; set; }
            public int ClienteId { get; set; }
            public int LibroId { get; set; }
            public DateTime? FechaCompra { get; set; }
            public int Cantidad { get; set; }
        }

        // PUT: api/Transacciones/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransaccion(int id, TransaccionUpdateDto transaccionDto)
        {
            if (id != transaccionDto.TransaccionId)
            {
                return BadRequest();
            }

            // Check if client exists
            if (!await _context.Clientes.AnyAsync(c => c.ClienteId == transaccionDto.ClienteId))
            {
                return BadRequest("El cliente especificado no existe");
            }

            // Check if book exists
            var libro = await _context.Libros.FindAsync(transaccionDto.LibroId);
            if (libro == null)
            {
                return BadRequest("El libro especificado no existe");
            }

            // Get existing transaction
            var transaccion = await _context.Transaccions.FindAsync(id);
            if (transaccion == null)
            {
                return NotFound();
            }

            // Update transaction properties
            transaccion.ClienteId = transaccionDto.ClienteId;
            transaccion.LibroId = transaccionDto.LibroId;
            if (transaccionDto.FechaCompra.HasValue)
            {
                transaccion.FechaCompra = transaccionDto.FechaCompra.Value;
            }
            transaccion.Cantidad = transaccionDto.Cantidad <= 0 ? 1 : transaccionDto.Cantidad;
            transaccion.Total = libro.Precio * transaccion.Cantidad;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransaccionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Transacciones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaccion(int id)
        {
            var transaccion = await _context.Transaccions.FindAsync(id);
            if (transaccion == null)
            {
                return NotFound();
            }

            _context.Transaccions.Remove(transaccion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TransaccionExists(int id)
        {
            return _context.Transaccions.Any(e => e.TransaccionId == id);
        }
    }
}