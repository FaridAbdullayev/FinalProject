using Service.Dtos.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderDto>> GetOrdersAsync(string userId);
        Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto, string userId);
        Task<OrderDto> GetOrderByIdAsync(int orderId, string userId);
    }
}
