using Core.Entities;
using Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class SettingRepository:Repository<Setting>,ISettingRepository
    {
        private readonly AppDbContext _appDbContext;
        public SettingRepository(AppDbContext context) : base(context)
        {
        }
    }
}
