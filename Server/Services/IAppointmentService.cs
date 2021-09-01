using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Services
{
    interface IAppointmentService
    {
        IEnumerable<Appointement> GetAppointements(string pageIndex, string pageSize);
        bool IsAppointmentExist(DateTime time);
        bool IsAppointmentInThePast(DateTime time);
        IEnumerable<Appointement> GetAppointementsByDate(string dateString);
        Task<IEnumerable<Appointement>> DeleteAppointement(int id);
        Task<string> PostAppointement(Appointement appointement);
        Task<string> PutAppointement(int id, Appointement appointement);
    }
}
