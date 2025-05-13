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

        // POST: api/Transacciones
        [HttpPost]
        public async Task<ActionResult<Transaccion>> PostTransaccion(Transaccion transaccion)
        {
            // Check if client exists
            if (!await _context.Clientes.AnyAsync(c => c.ClienteId == transaccion.ClienteId))
            {
                return BadRequest("El cliente especificado no existe");
            }

            // Check if book exists
            var libro = await _context.Libros.FindAsync(transaccion.LibroId);
            if (libro == null)
            {
                return BadRequest("El libro especificado no existe");
            }

            // Set purchase date to now if not provided
            if (transaccion.FechaCompra == default)
            {
                transaccion.FechaCompra = DateTime.Now;
            }

            // Set default quantity if not provided
            if (transaccion.Cantidad <= 0)
            {
                transaccion.Cantidad = 1;
            }

            // Calculate total based on book price and quantity
            transaccion.Total = libro.Precio * transaccion.Cantidad;

            _context.Transaccions.Add(transaccion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTransaccion", new { id = transaccion.TransaccionId }, transaccion);
        }

        // PUT: api/Transacciones/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransaccion(int id, Transaccion transaccion)
        {
            if (id != transaccion.TransaccionId)
            {
                return BadRequest();
            }

            // Check if client exists
            if (!await _context.Clientes.AnyAsync(c => c.ClienteId == transaccion.ClienteId))
            {
                return BadRequest("El cliente especificado no existe");
            }

            // Check if book exists
            var libro = await _context.Libros.FindAsync(transaccion.LibroId);
            if (libro == null)
            {
                return BadRequest("El libro especificado no existe");
            }

            // Set default quantity if not provided
            if (transaccion.Cantidad <= 0)
            {
                transaccion.Cantidad = 1;
            }

            // Calculate total based on book price and quantity
            transaccion.Total = libro.Precio * transaccion.Cantidad;

            _context.Entry(transaccion).State = EntityState.Modified;

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