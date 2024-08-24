using FluentValidation;
using Service.Dtos.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dtos.Users
{
    public class MemberRoomReviewDto
    {
        public int RoomId { get; set; }
        public string Text { get; set; }
        public byte Rate { get; set; }
    }


    public class MemberRoomReviewDtoValidator : AbstractValidator<MemberRoomReviewDto>
    {
        public MemberRoomReviewDtoValidator()
        {
            RuleFor(x => x.Text).NotEmpty().MaximumLength(50).MinimumLength(3);
            RuleFor(x => (int)x.Rate).InclusiveBetween(1, 5).WithMessage("Rate must be between 1 and 5.");
        }
    }
}
