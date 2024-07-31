using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dtos.OurStaff
{
    public class OurStaffUpdateDto
    {
        public string Name { get; set; }
        public string Position { get; set; }
        public string Description { get; set; }
        public IFormFile? File { get; set; }
    }
    public class OurStaffUpdateDtoValidator : AbstractValidator<OurStaffUpdateDto>
    {
        public OurStaffUpdateDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(50).MinimumLength(3);
            RuleFor(x => x.Position).NotEmpty().MaximumLength(50).MinimumLength(3);
            RuleFor(x => x.Description).NotEmpty().MinimumLength(3);

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
