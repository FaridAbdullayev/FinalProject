using Core.Entities;
using Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class BranchRepository:Repository<Branch>,IBranchRepository
    {
        private readonly AppDbContext _appDbContext;
        public BranchRepository(AppDbContext context) : base(context)
        {

        }
    }
}
