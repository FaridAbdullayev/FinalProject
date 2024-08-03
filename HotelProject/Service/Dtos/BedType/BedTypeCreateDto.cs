using FluentValidation;
using Service.Dtos.Branch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dtos.BedType
{
    public class BedTypeCreateDto
    {
        public string Name { get; set; }
    }
    public class BedTypeCreateDtoValidator : AbstractValidator<BranchCreateDto>
    {
        public BedTypeCreateDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(50).MinimumLength(3);
        }
    }
}
