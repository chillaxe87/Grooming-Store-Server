using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models
{
    public class Appointement
    {
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }
        public string UserId { get; set; }
        [Required]
        public DateTime ScheduledFor { get; set; }
        [Required]
        public DateTime ScheduledAt{ get; set; }
    }
}
