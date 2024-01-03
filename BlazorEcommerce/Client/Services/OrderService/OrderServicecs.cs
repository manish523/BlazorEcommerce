using Microsoft.AspNetCore.Components;

namespace BlazorEcommerce.Client.Services.OrderService
{
    public class OrderServicecs : IOrderService
    {
        private readonly HttpClient _http;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly NavigationManager _navigationManager;

        public OrderServicecs(HttpClient http, AuthenticationStateProvider authStateProvider, NavigationManager navigationManager)
        {
            _http = http;
            _authStateProvider = authStateProvider;
            _navigationManager = navigationManager;
        }

        public async Task<OrderDetailsDTO> GetOrderDetails(int orderId)
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<OrderDetailsDTO>>($"api/order/{orderId}");
            return result.Data;
        }

        public async Task<List<OrderOverViewDTO>> GetOrders()
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<List<OrderOverViewDTO>>>("api/order");
            return result.Data;
        }

        public async Task PlaceOredr()
        {
            if (await IsUserAuthenticated())
            {
                await _http.PostAsync("api/order", null);
            }
            else
            {
                _navigationManager.NavigateTo("login");
            }
        }

        private async Task<bool> IsUserAuthenticated()
        {
            return (await _authStateProvider.GetAuthenticationStateAsync()).User.Identity.IsAuthenticated;
        }
    }
}
