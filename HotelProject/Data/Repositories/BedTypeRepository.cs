using Core.Entities;
using Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class BedTypeRepository:Repository<BedType>,IBedTypeRepository
    {
        private readonly AppDbContext _appDbContext;
        public BedTypeRepository(AppDbContext context) : base(context)
        {

        }
    }
}
