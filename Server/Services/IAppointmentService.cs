using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Services
{
    interface IAppointmentService
    {
        IEnumerable<Appointement> GetAppointements(int page);
        bool IsAppointmentExist(DateTime time);
        bool IsAppointmentInThePast(DateTime time);
    }
}
