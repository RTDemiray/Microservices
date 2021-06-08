using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeCourse.Web.Models;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IBasketService _basketService;
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService, IBasketService basketService)
        {
            _orderService = orderService;
            _basketService = basketService;
        }

        public async Task<IActionResult> CheckOut()
        {
            var basket = await _basketService.Get();
            ViewBag.basket = basket;
            return View(new CheckoutInfoInput());
        }

        [HttpPost]
        public async Task<IActionResult> CheckOut(CheckoutInfoInput checkoutInfoInput)
        {
            // 1. yol senkron iletişim
            // var orderStatus = await _orderService.CreateOrder(checkoutInfoInput);
            
            // 2. yol asenkron iletişim
            var orderSuspend = await _orderService.SuspendOrder(checkoutInfoInput);
            if (!orderSuspend.IsSuccessfull)
            {
                var basket = await _basketService.Get();
                ViewBag.basket = basket;
                ViewBag.error = orderSuspend.Error;
                return View();
            }
            
            // 1. yol senkron iletişim
            // return RedirectToAction(nameof(SuccessFullCheckOut), new {orderId = orderStatus.OrderId});
            
            //2. yol asenkron iletişim
            return RedirectToAction(nameof(SuccessFullCheckOut), new {orderId = new Random().Next(1,1000)});
        }

        public IActionResult SuccessFullCheckOut(int orderId)
        {
            ViewBag.orderId = orderId;
            return View();
        }

        public async Task<IActionResult> CheckOutHistory()
        {
            return View(await _orderService.GetOrder());
        }
    }
}