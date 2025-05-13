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
    public class LibrosController : ControllerBase
    {
        private readonly LibraryContext _context;

        public LibrosController(LibraryContext context) 
        {
            _context = context;
        }

        // GET: api/Libros
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Libro>>> GetLibros()
        {
            return await _context.Libros
                .Include(l => l.Editorial)
                .ToListAsync();
        }

        // GET: api/Libros/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Libro>> GetLibro(int id)
        {
            var libro = await _context.Libros
                .Include(l => l.Editorial)
                .FirstOrDefaultAsync(l => l.LibroId == id);

            if (libro == null)
            {
                return NotFound();
            }

            return libro;
        }

        // GET: api/Libros/isbn/9781234567890
        [HttpGet("isbn/{isbn}")]
        public async Task<ActionResult<Libro>> GetLibroByIsbn(string isbn)
        {
            var libro = await _context.Libros
                .Include(l => l.Editorial)
                .FirstOrDefaultAsync(l => l.Isbn == isbn);

            if (libro == null)
            {
                return NotFound();
            }

            return libro;
        }

        // POST: api/Libros
        [HttpPost]
        public async Task<ActionResult<Libro>> PostLibro(Libro libro)
        {
            // Check if editorial exists
            if (!await _context.Editorials.AnyAsync(e => e.EditorialId == libro.EditorialId))
            {
                return BadRequest("La editorial especificada no existe");
            }

            // Check if ISBN is already used
            if (!string.IsNullOrEmpty(libro.Isbn) && await _context.Libros.AnyAsync(l => l.Isbn == libro.Isbn))
            {
                return BadRequest("Ya existe un libro con el mismo ISBN");
            }

            _context.Libros.Add(libro);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLibro", new { id = libro.LibroId }, libro);
        }

        // PUT: api/Libros/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLibro(int id, Libro libro)
        {
            if (id != libro.LibroId)
            {
                return BadRequest();
            }

            // Check if editorial exists
            if (!await _context.Editorials.AnyAsync(e => e.EditorialId == libro.EditorialId))
            {
                return BadRequest("La editorial especificada no existe");
            }

            // Check if ISBN is already used by another book
            if (!string.IsNullOrEmpty(libro.Isbn) && await _context.Libros.AnyAsync(l => l.Isbn == libro.Isbn && l.LibroId != id))
            {
                return BadRequest("Ya existe otro libro con el mismo ISBN");
            }

            _context.Entry(libro).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LibroExists(id))
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

        // DELETE: api/Libros/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLibro(int id)
        {
            var libro = await _context.Libros.FindAsync(id);
            if (libro == null)
            {
                return NotFound();
            }

            // Check if book has transactions
            bool hasTransactions = await _context.Transaccions.AnyAsync(t => t.LibroId == id);
            if (hasTransactions)
            {
                return BadRequest("No se puede eliminar el libro porque tiene transacciones asociadas");
            }

            _context.Libros.Remove(libro);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LibroExists(int id)
        {
            return _context.Libros.Any(e => e.LibroId == id);
        }
    }
}