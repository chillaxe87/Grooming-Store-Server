using Microsoft.EntityFrameworkCore;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Services
{
    public class AppointmentsService : IAppointmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly int defaultPageSize = 8;
        public AppointmentsService (ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Appointement>> DeleteAppointement(int id)
        {
            var appointement = await _context.Appointements.FindAsync(id);
            if (appointement == null)
            {
                return null;
            }

            _context.Appointements.Remove(appointement);
            await _context.SaveChangesAsync();
      
            return (IEnumerable<Appointement>) appointement;
        }

        public IEnumerable<Appointement> GetAppointements(string page, string size)
        {
            int pageIndex = page == "" ?  1 : int.Parse(page);
            pageIndex = pageIndex < 1 ? 1 : pageIndex;

            int pageSize = size == "" ? defaultPageSize : int.Parse(size);


            IEnumerable<Appointement> result = _context.Appointements
                .Where(t => t.ScheduledFor.DayOfYear >= DateTime.Now.DayOfYear)
                .OrderBy(a => a.ScheduledFor)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize + 1);



            return result; 
        }
        public IEnumerable<Appointement> GetAppointementsByDate(string dateString)
        {
            string[] val = dateString.Split('.');
            DateTime date = new(int.Parse(val[0]), int.Parse(val[1]), int.Parse(val[2]));
            IEnumerable<Appointement> result = _context.Appointements.Where(t => t.ScheduledFor.Date == date.Date);

            return result;
        }

        public bool IsAppointmentExist(DateTime time)
        {
            var result = _context.Appointements.FirstOrDefault(a => a.ScheduledFor == time);
            return result != null;
        }

        public bool IsAppointmentInThePast(DateTime time)
        {
            return DateTime.Now > time;
        }

        public async Task<string> PostAppointement(Appointement appointement)
        {
            appointement.ScheduledFor = appointement.ScheduledFor.AddHours(3);
            appointement.ScheduledAt = appointement.ScheduledAt.AddHours(3);
            var time1 = appointement.ScheduledFor;
            var time2 = appointement.ScheduledAt;
            if (IsAppointmentExist(appointement.ScheduledFor))
            {
                return "Requested appointment is already taken please try again";
            }
            if (IsAppointmentInThePast(appointement.ScheduledFor))
            {
                return "Cannot schedule appointment in the past";
            }
      
            _context.Appointements.Add(appointement);

            await _context.SaveChangesAsync();

            return "Created";

        }

        public async Task<string> PutAppointement(int id, Appointement appointement)
        {
            appointement.ScheduledFor = appointement.ScheduledFor.AddHours(3);
            appointement.ScheduledAt = appointement.ScheduledAt.AddHours(3);
            if (IsAppointmentInThePast(appointement.ScheduledFor))
            {
                return "Cannot schedule appointment in the past";

            }
            if (IsAppointmentExist(appointement.ScheduledFor))
            {
                return "Appointment is already taken";
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
                    return "Not Found";
                }
                else
                {
                    throw;
                }
            }
            return "Updated";
        }
        private bool AppointementExists(int id)
        {
            return _context.Appointements.Any(e => e.Id == id);
        }
    }
}
