using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dtos.Users
{
    public class CreateOrderDto
    {
        public int ReservationId { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
