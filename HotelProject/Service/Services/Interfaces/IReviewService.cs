using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Service.Dtos.Review;
using Service.Dtos;
using Service.Dtos.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.Dtos.Contact;
using Core.Entities.Enum;

namespace Service.Services.Interfaces
{
    public interface IReviewService
    {
        Task<int> MemberReview(MemberRoomReviewDto review,string userId);

        PaginatedList<ReviewGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10);
        List<ReviewListItemGetDto> GetAll();

        void UpdateOrderStatus(int id, ReviewStatus newStatus);
        ReviewDetailDto GetById(int id);

    }
}
