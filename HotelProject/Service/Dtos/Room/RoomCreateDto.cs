using Core.Entities.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dtos.Room
{
    public class RoomCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public double Area { get; set; }
        public int MaxChildrenCount { get; set; }
        public int MaxAdultsCount { get; set; }
        public BedType BedType { get; set; }
        public int BranchId { get; set; }
        public List<IFormFile> Images {  get; set; } = new List<IFormFile>();
        public List<int> ServiceIds { get; set; } = new List<int>();
    }

    public class RoomCreateDtoValidator : AbstractValidator<RoomCreateDto>
    {
        public RoomCreateDtoValidator()
        {
            //RuleFor(x => x.Name).NotEmpty().MaximumLength(50).MinimumLength(3);
            //RuleFor(x => x.Description).NotEmpty().MaximumLength(50).MinimumLength(5);
        }
    }
}
