using Core.Entities;
using Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class ReservationRepository:Repository<Reservation>,IReservationRepository
    {
        private readonly AppDbContext _appDbContext;
        public ReservationRepository(AppDbContext context) : base(context)
        {

        }
    }
}
