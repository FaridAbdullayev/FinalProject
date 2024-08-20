using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dtos.Branch
{
    public class BranchIncome
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public double Income { get; set; }
    }
}
