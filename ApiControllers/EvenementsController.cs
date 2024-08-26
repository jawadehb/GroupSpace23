using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GroupSpace23.Data;
using GroupSpace23.Models;
using Microsoft.AspNetCore.Authorization;

namespace GroupSpace23.ApiControllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class EvenementsController : ControllerBase
    {
        private readonly MyDbContext _context;

        public EvenementsController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/Evenements
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Evenement>>> GetEvenements()
        {
            if (_context.Evenements == null)
            {
                return NotFound();
            }
            return await _context.Evenements.Where(g => g.Ended > DateTime.Now).ToListAsync();
        }

        // GET: api/Evenements/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Evenement>> GetEvenement(int id)
        {
            if (_context.Evenements == null)
            {
                return NotFound();
            }
            var @evenement = await _context.Evenements.FindAsync(id);

            if (@evenement == null)
            {
                return NotFound();
            }

            return @evenement;
        }



        // PUT: api/Evenements/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvenement(int id, Evenement @evenement)
        {
            if (id != @evenement.Id)
            {
                return BadRequest();
            }

            _context.Entry(@evenement).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EvenementExists(id))
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

        // POST: api/Evenements
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Evenement>> PostEvenement(Evenement @evenement)
        {
            if (_context.Evenements == null)
            {
                return Problem("Entity set 'MyDbContext.Evenements'  is null.");
            }
            _context.Evenements.Add(@evenement);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEvenement", new { id = @evenement.Id }, @evenement);
        }

        // DELETE: api/Evenements/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvenement(int id)
        {
            if (_context.Evenements == null)
            {
                return NotFound();
            }
            var @evenement = await _context.Evenements.FindAsync(id);
            if (@evenement == null)
            {
                return NotFound();
            }

            //_context.Evenements.Remove(@evenement);
            evenement.Ended = DateTime.Now;
            _context.Update(@evenement);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EvenementExists(int id)
        {
            return (_context.Evenements?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}