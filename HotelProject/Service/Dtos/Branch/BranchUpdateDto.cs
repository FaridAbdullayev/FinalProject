using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dtos.Branch
{
    public class BranchUpdateDto
    {
        public string Name { get; set; }
    }

    public class BranchUpdateDtoValidator : AbstractValidator<BranchUpdateDto>
    {
        public BranchUpdateDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(50).MinimumLength(3);
        }
    }
}
