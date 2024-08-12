using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dtos.Users
{
    public class MemberForgetPasswordDto
    {
        public string Email { get; set; }
    }

    public class MemberForgetPasswordDtoValidator : AbstractValidator<MemberForgetPasswordDto>
    {
        public MemberForgetPasswordDtoValidator()
        {

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.").MinimumLength(3).MaximumLength(100)
                .EmailAddress().WithMessage("Invalid email format.");


        }
    }
}
