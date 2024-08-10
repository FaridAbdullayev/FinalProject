using Core.Entities;
using Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class RoomReviewRepository:Repository<RoomReview>,IRoomReviewRepository
    {
        private readonly AppDbContext _appDbContext;
        public RoomReviewRepository(AppDbContext context) : base(context)
        {

        }
    }
}
