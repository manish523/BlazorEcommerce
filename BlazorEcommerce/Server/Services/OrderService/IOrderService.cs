﻿using BlazorEcommerce.Shared.DTOs;

namespace BlazorEcommerce.Server.Services.OrderService
{
    public interface IOrderService
    {
        Task<ServiceResponse<bool>> PlaceOrder(int userId);
        Task<ServiceResponse<List<OrderOverViewDTO>>> GetOrders();
        Task<ServiceResponse<OrderDetailsDTO>> GetOrdersDetails(int orderId);
    }
}
