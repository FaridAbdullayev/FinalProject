using Core.Entities;
using Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class ContactRepository:Repository<Contact>,IContactRepository
    {
        private readonly AppDbContext _appDbContext;
        public ContactRepository(AppDbContext context) : base(context)
        {

        }
    }
}
