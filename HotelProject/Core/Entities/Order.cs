using Core.Entities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Order:BaseEntity
    {
        public DateTime OrderDate { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderStatus Status { get; set; }
    }
}
