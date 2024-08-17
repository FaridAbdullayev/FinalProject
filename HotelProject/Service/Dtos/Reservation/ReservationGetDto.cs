using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dtos.Reservation
{
    public class ReservationGetDto
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? UserName { get; set; }
        public string RoomName { get; set; }
        public string Status { get; set; }
    }
}
