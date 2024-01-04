using Stripe;
using Stripe.Checkout;

namespace BlazorEcommerce.Server.Services.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly ICartService _cartService;
        private readonly IAuthService _authService;
        private readonly IOrderService _orderService;

        const string secret = "whsec_e526922bc6f6a1667787169db956dd51c3fd85b72a557d56d7ae4eb3871810f1";

        public PaymentService(ICartService cartService, IAuthService authService, IOrderService orderService)
        {
            StripeConfiguration.ApiKey = "sk_test_51HhssoG08nv2dgac0L7G7mapSFi7HOsDEvuM40tgBaH3xTJJe9nKYWH2tXCXjlMGswCG6LXzMmpcy8CxeuDBU3Gt00BA2ufbwi";

            _cartService = cartService;
            _authService = authService;
            _orderService = orderService;
        }
        ////string secretKey = "sk_test_51HhssoG08nv2dgac0L7G7mapSFi7HOsDEvuM40tgBaH3xTJJe9nKYWH2tXCXjlMGswCG6LXzMmpcy8CxeuDBU3Gt00BA2ufbwi";
        ////string publishableKey = "pk_test_51HhssoG08nv2dgachUxajm2JqmN1lezrsYxgHpgCLDuYMIsHenaHJbKPCVSseyyopU8VBOWdXirid1v1qIhZp8YD00u6fyJDri";
        public async Task<Session> CreateCheckoutSession()
        {
            var products = (await _cartService.GetDbCartProducts()).Data;
            var lineItems = new List<SessionLineItemOptions>();
            products.ForEach(product => lineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmountDecimal = product.Price * 100,
                    Currency = "inr",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = product.Title,
                        Images = new List<string> { product.ImageUrl }
                    }
                },
                Quantity = product.Quantity,
            }));

            var options = new SessionCreateOptions
            {
                CustomerEmail = _authService.GetUserEmail(),
                ShippingAddressCollection = new SessionShippingAddressCollectionOptions
                {
                    AllowedCountries = new List<string>
                    {
                        "IN"
                    },
                },
                PaymentMethodTypes = new List<string>
                {
                    "card"
                },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = "https://localhost:7271/order-success",
                CancelUrl = "https://localhost:7271/cart"
            };

            var service = new SessionService();
            Session session = service.Create(options);
            return session;
        }

        public async Task<ServiceResponse<bool>> FulfillOrder(HttpRequest request)
        {
            var json = await new StreamReader(request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    request.Headers["Stripe-Signature"],
                    secret
                    );

                if (stripeEvent.Type == Events.CheckoutSessionCompleted)
                {
                    var session = stripeEvent.Data.Object as Session;
                    var user = await _authService.GetUserByEmail(session.CustomerEmail);
                    await _orderService.PlaceOrder(user.Id);
                }

                return new ServiceResponse<bool> { Success = true };
            }
            catch (Exception e)
            {

                return new ServiceResponse<bool> { Data = false, Success = false, Message = e.Message };
            }
        }
    }
}
