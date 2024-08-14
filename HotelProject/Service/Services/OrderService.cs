using Core.Entities;
using Core.Entities.Enum;
using Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Service.Dtos;
using Service.Dtos.Users;
using Service.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly UserManager<AppUser> _userManager;

        public OrderService(IOrderRepository orderRepository, UserManager<AppUser> userManager)
        {
            _orderRepository = orderRepository;
            _userManager = userManager;
        }

        public async Task<List<OrderDto>> GetOrdersAsync(string userId)
        {
            var orders = _orderRepository.GetAll(o => o.AppUserId == userId);
            return orders.Select(o => new OrderDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                TotalPrice = o.TotalPrice,
                Status = o.Status.ToString()
            }).ToList();
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto, string userId)
        {
            var order = new Order
            {
                OrderDate = DateTime.UtcNow,
                AppUserId = userId,
                ReservationId = createOrderDto.ReservationId,
                TotalPrice = createOrderDto.TotalPrice,
                Status = OrderStatus.Pending
            };

             _orderRepository.Add(order);
             _orderRepository.Save();

            return new OrderDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                Status = order.Status.ToString()
            };
        }

        public async Task<OrderDto> GetOrderByIdAsync(int orderId, string userId)
        {
            var order = _orderRepository.Get(o => o.Id == orderId && o.AppUserId == userId);
            if (order == null)
            {
                throw new Exception("Order not found.");
            }

            return new OrderDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                Status = order.Status.ToString()
            };
        }
    }
}
