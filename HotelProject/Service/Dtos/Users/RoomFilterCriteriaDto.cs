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
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<int> ServiceIds { get; set; } = new List<int>();
        public int? BranchId { get; set; }
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }
        public int? AdultsCount { get; set; }
        public int? ChildrenCount { get; set; }
    }

    public class RoomFilterCriteriaDtoValidator : AbstractValidator<RoomFilterCriteriaDto>
    {
        public RoomFilterCriteriaDtoValidator()
        {
            RuleFor(x => x.StartDate).NotEmpty();
            RuleFor(x => x.EndDate).NotEmpty();
            RuleFor(x => x.MinPrice).GreaterThanOrEqualTo(0).When(x => x.MinPrice.HasValue);
            RuleFor(x => x.MaxPrice).GreaterThanOrEqualTo(0).When(x => x.MaxPrice.HasValue);
            RuleFor(x => x.MaxPrice).GreaterThanOrEqualTo(x => x.MinPrice).When(x => x.MinPrice.HasValue && x.MaxPrice.HasValue);

            RuleFor(x => x.AdultsCount).NotNull().GreaterThanOrEqualTo(0);
            RuleFor(x => x.ChildrenCount).NotNull().GreaterThanOrEqualTo(0);
        }
    }

}
