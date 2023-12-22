using BlazorEcommerce.Shared.DTOs;

namespace BlazorEcommerce.Server.Services.ProductService
{
    public interface IProductService
    {
        Task<ServiceResponse<List<Product>>> GetProductsAsync();
        Task<ServiceResponse<Product>> GerProductAsync(int productId);
        Task<ServiceResponse<List<Product>>> GetProductsByCategory(string categoryUrl);
        Task<ServiceResponse<ProductSearchResultDTO>> SearchProducts(string searchText, int page);
        Task<ServiceResponse<List<string>>> GetProductSearchSuggestions(string searchString);
        Task<ServiceResponse<List<Product>>> GetFeaturedProducts();
    }
}
