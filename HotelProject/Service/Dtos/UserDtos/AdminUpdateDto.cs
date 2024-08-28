using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dtos.UserDtos
{
    public class AdminUpdateDto
    {
        public string UserName { get; set; }

        public string? CurrentPassword { get; set; }

        public string? NewPassword { get; set; }

        public string? ConfirmPassword { get; set; }
    }
    public class AdminUpdateDtoValidator : AbstractValidator<AdminUpdateDto>
    {
        public AdminUpdateDtoValidator()
        {
            RuleFor(x => x.UserName).NotNull().MaximumLength(50).MinimumLength(3);

            RuleFor(x => x.CurrentPassword).NotNull().MaximumLength(50).MinimumLength(8);

            RuleFor(x => x.NewPassword).NotNull().MaximumLength(50).MinimumLength(8);

            RuleFor(x => x.ConfirmPassword).NotNull().MaximumLength(50).MinimumLength(8);

            RuleFor(x => x)
               .Must(x => x.NewPassword == x.ConfirmPassword)
               .WithMessage("New password and confirm password must be the same.");
        }
    }
}
