using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dtos.Setting
{
    public class SettingUpdateDto
    {
        public string Value { get; set; }
    }
    public class SettingUpdateDtoValidator : AbstractValidator<SettingUpdateDto>
    {
        public SettingUpdateDtoValidator()
        {
            RuleFor(x => x.Value).NotEmpty().MinimumLength(1);
        }
    }
}
