using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dtos.Reservation
{
    public class MonthlyIncomeDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public double TotalIncome { get; set; }
    }
}
