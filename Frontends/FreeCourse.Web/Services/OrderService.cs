using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FreeCourse.Shared;
using FreeCourse.Shared.Service;
using FreeCourse.Web.Models;
using FreeCourse.Web.Services.Interfaces;
using MapsterMapper;

namespace FreeCourse.Web.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;
        private readonly IPaymentService _paymentService;
        private readonly IBasketService _basketService;
        private readonly ISharedIdentityService _sharedIdentityService;
        private readonly IMapper _mapper;
        
        public OrderService(IPaymentService paymentService, HttpClient httpClient, IBasketService basketService, IMapper mapper, ISharedIdentityService sharedIdentityService)
        {
            _paymentService = paymentService;
            _httpClient = httpClient;
            _basketService = basketService;
            _mapper = mapper;
            _sharedIdentityService = sharedIdentityService;
        }

        public async Task<OrderCreatedViewModel> CreateOrder(CheckoutInfoInput checkoutInfoInput)
        {
            var basket = await _basketService.Get();
            var paymentInfoInput = _mapper.Map<PaymentInfoInput>(checkoutInfoInput);
            paymentInfoInput.TotalPrice = basket.TotalPrice;

            var responsePayment = await _paymentService.RecivePayment(paymentInfoInput);
            if (!responsePayment)
            {
                return new OrderCreatedViewModel {Error = "Ödeme alınamadı", IsSuccessfull = false};
            }

            var orderCreateInput = new OrderCreateInput
            {
                BuyerId = _sharedIdentityService.GetUserId,
                Address = new AddressCreateInput{Province = checkoutInfoInput.Province,District = checkoutInfoInput.District,Street = checkoutInfoInput.Street, Line = checkoutInfoInput.Line, ZipCode = checkoutInfoInput.ZipCode}
            };
            basket.BasketItems.ForEach(x =>
            {
                var orderItem = new OrderItemCreateInput{ProductId = x.CourseId,Price = x.GetCurrentPrice,PictureUrl = "",ProductName = x.CourseName};
                orderCreateInput.OrderItems.Add(orderItem);
            });

            var response = await _httpClient.PostAsJsonAsync("orders", orderCreateInput);
            if (!response.IsSuccessStatusCode)
            {
                return new OrderCreatedViewModel {Error = "Sipariş oluşturulamadı", IsSuccessfull = false};
            }
            var orderCreatedViewModel = await response.Content.ReadFromJsonAsync<Response<OrderCreatedViewModel>>();
            orderCreatedViewModel.Data.IsSuccessfull = true;
            await _basketService.Delete();
            return orderCreatedViewModel.Data;
        }

        public async Task<OrderSuspendViewModel> SuspendOrder(CheckoutInfoInput checkoutInfoInput)
        {
            var basket = await _basketService.Get();
            
            var orderCreateInput = new OrderCreateInput
            {
                BuyerId = _sharedIdentityService.GetUserId,
                Address = new AddressCreateInput{Province = checkoutInfoInput.Province,District = checkoutInfoInput.District,Street = checkoutInfoInput.Street, Line = checkoutInfoInput.Line, ZipCode = checkoutInfoInput.ZipCode}
            };
            basket.BasketItems.ForEach(x =>
            {
                var orderItem = new OrderItemCreateInput{ProductId = x.CourseId,Price = x.GetCurrentPrice,PictureUrl = "",ProductName = x.CourseName};
                orderCreateInput.OrderItems.Add(orderItem);
            });
            
            var paymentInfoInput = _mapper.Map<PaymentInfoInput>(checkoutInfoInput);
            paymentInfoInput.TotalPrice = basket.TotalPrice;
            paymentInfoInput.Order = orderCreateInput;
            
            var responsePayment = await _paymentService.RecivePayment(paymentInfoInput);
            if (!responsePayment)
            {
                return new OrderSuspendViewModel {Error = "Ödeme alınamadı", IsSuccessfull = false};
            }
            
            var response = await _httpClient.PostAsJsonAsync("orders", orderCreateInput);
            if (!response.IsSuccessStatusCode)
            {
                return new OrderSuspendViewModel {Error = "Sipariş oluşturulamadı", IsSuccessfull = false};
            }
            var orderSuspendViewModel = await response.Content.ReadFromJsonAsync<Response<OrderSuspendViewModel>>();
            orderSuspendViewModel.Data.IsSuccessfull = true;
            await _basketService.Delete();
            return orderSuspendViewModel.Data;
        }

        public async Task<List<OrderViewModel>> GetOrder()
        {
            var response = await _httpClient.GetFromJsonAsync<Response<List<OrderViewModel>>>("orders");
            return response.Data;
        }
    }
}