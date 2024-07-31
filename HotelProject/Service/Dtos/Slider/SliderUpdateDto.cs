using FluentValidation;
using Microsoft.AspNetCore.Http;
using Service.Dtos.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dtos.Slider
{
    public class SliderUpdateDto
    {
        public IFormFile? File { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
    }

    public class SliderUpdateDtoValidator : AbstractValidator<SliderUpdateDto>
    {
        public SliderUpdateDtoValidator()
        {
            RuleFor(x => x.Description).NotEmpty().MaximumLength(50).MinimumLength(3);

            RuleFor(x => x).Custom((f, c) =>
            {
                if (f.File != null && f.File.Length > 2 * 1024 * 1024)
                {
                    c.AddFailure("File", "File must be less or equal than 2MB");
                }
            });
        }
    }
}
