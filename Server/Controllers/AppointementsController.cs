using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Models;
using Server.Services;



// drop all _context operations to services
namespace Server.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class AppointementsController : ControllerBase
    {
        private readonly AppointmentsService _repository;

        public AppointementsController(AppointmentsService appointmentsService)
        {
            _repository = appointmentsService;
        }


        //[AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<Appointement>> PostAppointement(Appointement appointement)
        {
            string res = await _repository.PostAppointement(appointement);

            if (res == "Created")
            {
                return StatusCode(201, appointement);
            }
            return StatusCode(400, new { message = res });
        }

        // GET: api/Appointements
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<Appointement>>> GetAppointementsAsync([FromQuery(Name = "page")] string page, [FromQuery(Name = "date")] string date, [FromQuery(Name = "size")] string pageSize)
        {

            if(date != null)
            {
                return await Task.Run(() => _repository.GetAppointementsByDate(date).ToList());
            }
            page = page ?? "";
            pageSize = pageSize ?? "";
            return await Task.Run(()=>_repository.GetAppointements(page, pageSize).ToList());

        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppointement(int id, Appointement appointement)
        {
            string res = await _repository.PutAppointement(id, appointement);
            if (res == "Updated")
            {
                return StatusCode(200, appointement);
            }

            return StatusCode(405, new { message = res });
        }

        //[AllowAnonymous]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointement(int id)
        {
            var result = await _repository.DeleteAppointement(id);
            if(result == null)
                return StatusCode(400, "Not Found");
           
            return StatusCode(200, new { message = "Deleted" });
        }
    }
}
