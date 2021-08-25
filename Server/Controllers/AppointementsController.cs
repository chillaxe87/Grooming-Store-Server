using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Models;
using Server.Services;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class AppointementsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly AppointmentsService _appointmentsService;

        public AppointementsController(ApplicationDbContext context, AppointmentsService appointmentsService)
        {
            _context = context;
            _appointmentsService = appointmentsService;
        }

        // GET: api/Appointements
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<Appointement>>> GetAppointements([FromQuery(Name = "page")] string page)
        {
            int index = page != null ? int.Parse(page) : 1;
            index = index < 1 ? 1 : index;
            var appointmentsList = _appointmentsService.GetAppointements(index).ToList();
            await _context.Appointements.Where(t => t.ScheduledFor > DateTime.Now.AddHours(3)).OrderBy(x => x.ScheduledFor).ToListAsync();
            return appointmentsList;
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
            appointement.ScheduledFor = appointement.ScheduledFor.AddHours(3);
            appointement.ScheduledAt = appointement.ScheduledAt.AddHours(3);
            if (_appointmentsService.IsAppointmentInThePast(appointement.ScheduledFor))
            {
                return Content("Cannot schedule appointment in the past");
            }
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

            return Ok();
        }

        // POST: api/Appointements
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        //[AllowAnonymous]
        public async Task<ActionResult<Appointement>> PostAppointement(Appointement appointement)
        {
            appointement.ScheduledFor = appointement.ScheduledFor.AddHours(3);
            appointement.ScheduledAt = appointement.ScheduledAt.AddHours(3);

            if (_appointmentsService.IsAppointmentExist(appointement.ScheduledFor))
            {
                return Content("Requested appointment is already taken please try again");
            }
            if (_appointmentsService.IsAppointmentInThePast(appointement.ScheduledFor))
            {
                return Content("Cannot schedule appointment in the past");
            }
            _context.Appointements.Add(appointement);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAppointement", new { id = appointement.Id }, appointement);
        }

        // DELETE: api/Appointements/5
        [HttpDelete("{id}")]
        //[AllowAnonymous]
        public async Task<IActionResult> DeleteAppointement(int id)
        {
            var appointement = await _context.Appointements.FindAsync(id);
            if (appointement == null)
            {
                return NotFound();
            }

            _context.Appointements.Remove(appointement);
            await _context.SaveChangesAsync();

            return Content("deleted");
        }

        private bool AppointementExists(int id)
        {
            return _context.Appointements.Any(e => e.Id == id);
        }
    }
}
