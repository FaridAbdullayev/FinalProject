

using HotelUI.Models;
using HotelUI.Models.Admin;
using HotelUI.Models.Contact;
using HotelUI.Models.Reservation;

namespace HotelUI.Services
{
    public interface ICrudService
    {
        Task<PaginatedResponse<TResponse>> GetAllPaginated<TResponse>(string path, int page);
        Task<TResponse> Get<TResponse>(string path);

        Task<CreateResponse> Create<TRequest>(TRequest request, string path);
        Task Update<TRequest>(TRequest request, string path);
        Task Delete(string path);

        Task<CreateResponse> CreateFromForm<TRequest>(TRequest request, string path);
        Task UpdateFormForm<TRequest>(TRequest request, string path);

        Task<CreateResponseForAdmin> CreateForAdmins<TRequest>(TRequest request, string path);

        Task SendMessageToUser(AdminAndIUserInteraction interaction);

        Task Status(string path);

        Task<TResponse> GetAsyncBranchesIncome<TResponse>(string endpoint);

        Task<Dictionary<string, int>> GetOrdersPricePerYearAsync();
    }
}
