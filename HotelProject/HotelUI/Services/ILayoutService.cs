using HotelUI.Models.Admin;

namespace HotelUI.Services
{
    public interface ILayoutService
    {
        Task<UserProfileResponse> GetProfile();
    }
}
