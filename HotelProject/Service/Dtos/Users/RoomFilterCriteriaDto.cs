using FluentValidation;
using Service.Dtos.Slider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dtos.Users
{
    public class RoomFilterCriteriaDto
    {
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; }
        public List<int>? ServiceIds { get; set; } = new List<int>();
        public int? BranchId { get; set; }
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }
        public int MaxAdultsCount { get; set; }
        public int MaxChildrenCount { get; set; }
    }

    public class RoomFilterCriteriaDtoValidator : AbstractValidator<RoomFilterCriteriaDto>
    {
        public RoomFilterCriteriaDtoValidator()
        {
            RuleFor(x => x.StartDate).NotEmpty();
            RuleFor(x => x.EndDate).NotEmpty();
            RuleFor(x => x.MaxAdultsCount).NotNull();
            RuleFor(x => x.MaxChildrenCount).NotNull();
        }
    }

}
