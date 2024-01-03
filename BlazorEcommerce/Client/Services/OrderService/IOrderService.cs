namespace BlazorEcommerce.Client.Services.OrderService
{
    public interface IOrderService
    {
        Task PlaceOredr();
        Task<List<OrderOverViewDTO>> GetOrders();
        Task<OrderDetailsDTO> GetOrderDetails(int orderId);
    }
}
