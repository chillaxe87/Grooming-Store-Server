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
        private readonly int pageSize = 18;
        public AppointmentsService (ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Appointement> GetAppointements(int appointmentsPage = 1)
        {
            IEnumerable<Appointement> result = _context.Appointements
                .Where(t => t.ScheduledFor > DateTime.Now.AddHours(3))
                .OrderBy(a => a.ScheduledFor)
                .Skip((appointmentsPage - 1) * pageSize)
                .Take(pageSize);

            return result;
        }

        public bool IsAppointmentExist(DateTime time)
        {
            var result = _context.Appointements.FirstOrDefault(a => a.ScheduledFor == time);
            return result != null;
        }

        public bool IsAppointmentInThePast(DateTime time)
        {
            return DateTime.Now.AddHours(3) > time;
        }
    }
}
