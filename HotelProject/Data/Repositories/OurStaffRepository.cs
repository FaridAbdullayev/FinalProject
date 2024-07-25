using Core.Entities;
using Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class OurStaffRepository : Repository<OurStaff>, IOurStaffRepository
    {
        private readonly AppDbContext _appDbContext;
        public OurStaffRepository(AppDbContext context) : base(context)
        {

        }
    }
}
