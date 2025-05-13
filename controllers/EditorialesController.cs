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
    public class EditorialesController : ControllerBase
    {
        private readonly LibraryContext _context;

        public EditorialesController(LibraryContext context)
        {
            _context = context;
        }

        // GET: api/Editoriales
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Editorial>>> GetEditoriales()
        {
            return await _context.Editorials.ToListAsync();
        }

        // GET: api/Editoriales/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Editorial>> GetEditorial(int id)
        {
            var editorial = await _context.Editorials.FindAsync(id);

            if (editorial == null)
            {
                return NotFound();
            }

            return editorial;
        }

        // GET: api/Editoriales/5/libros
        [HttpGet("{id}/libros")]
        public async Task<ActionResult<IEnumerable<Libro>>> GetEditorialLibros(int id)
        {
            if (!EditorialExists(id))
            {
                return NotFound();
            }

            return await _context.Libros
                .Where(l => l.EditorialId == id)
                .ToListAsync();
        }

        // POST: api/Editoriales
        [HttpPost]
        public async Task<ActionResult<Editorial>> PostEditorial(Editorial editorial)
        {
            _context.Editorials.Add(editorial);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEditorial", new { id = editorial.EditorialId }, editorial);
        }

        // PUT: api/Editoriales/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEditorial(int id, Editorial editorial)
        {
            if (id != editorial.EditorialId)
            {
                return BadRequest();
            }

            _context.Entry(editorial).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EditorialExists(id))
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

        // DELETE: api/Editoriales/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEditorial(int id)
        {
            var editorial = await _context.Editorials.FindAsync(id);
            if (editorial == null)
            {
                return NotFound();
            }

            // Check if editorial has books
            bool hasBooks = await _context.Libros.AnyAsync(l => l.EditorialId == id);
            if (hasBooks)
            {
                return BadRequest("No se puede eliminar la editorial porque tiene libros asociados");
            }

            _context.Editorials.Remove(editorial);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EditorialExists(int id)
        {
            return _context.Editorials.Any(e => e.EditorialId == id);
        }
    }
}