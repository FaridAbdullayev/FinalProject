using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dtos.UserDtos
{
    public class AdminCreateDto
    {
        public string UserName {  get; set; }
        public string Password { get; set; }
    }

    public class AdminCreateDtoValidator : AbstractValidator<AdminCreateDto>
    {
        public  AdminCreateDtoValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().MaximumLength(50).MinimumLength(3);

            RuleFor(x => x.Password).NotEmpty().MaximumLength(50).MinimumLength(3);

        }
    }
}
