using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class AppointementsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AppointementsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Appointements
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Appointement>>> GetAppointements()
        {
            return await _context.Appointements.ToListAsync();
        }

        // GET: api/Appointements/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Appointement>> GetAppointement(int id)
        {
            var appointement = await _context.Appointements.FindAsync(id);

            if (appointement == null)
            {
                return NotFound();
            }

            return appointement;
        }

        // PUT: api/Appointements/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppointement(int id, Appointement appointement)
        {
            appointement.Id = id;

            _context.Entry(appointement).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppointementExists(id))
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

        // POST: api/Appointements
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Appointement>> PostAppointement(Appointement appointement)
        {
            _context.Appointements.Add(appointement);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAppointement", new { id = appointement.Id }, appointement);
        }

        // DELETE: api/Appointements/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointement(int id)
        {
            var appointement = await _context.Appointements.FindAsync(id);
            if (appointement == null)
            {
                return NotFound();
            }

            _context.Appointements.Remove(appointement);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AppointementExists(int id)
        {
            return _context.Appointements.Any(e => e.Id == id);
        }
    }
}
