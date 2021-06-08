using System;
using System.Net;
using System.Threading.Tasks;
using FreeCourse.Service.FakePayment.Models;
using FreeCourse.Shared.ControllerBases;
using FreeCourse.Shared.Dtos;
using FreeCourse.Shared.Messages;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Service.FakePayment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FakePayment : CustomBaseController
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public FakePayment(ISendEndpointProvider sendEndpointProvider)
        {
            _sendEndpointProvider = sendEndpointProvider;
        }

        [HttpPost]
        public async Task<IActionResult> ReceivePayment(PaymentDto paymentDto)
        {
            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:create-order-service")); //kuyruk ismi tanımlıyoruz.

            var createOrderMessageCommand = new CreateOrderMessageCommand
            {
                BuyerId = paymentDto.Order.BuyerId,
                District = paymentDto.Order.Address.District,
                Line = paymentDto.Order.Address.Line,
                Province = paymentDto.Order.Address.Province,
                Street = paymentDto.Order.Address.Street,
                ZipCode = paymentDto.Order.Address.ZipCode
            };
            
            paymentDto.Order.OrderItems.ForEach(x =>
            {
                createOrderMessageCommand.OrderItems.Add(new OrderItem
                {
                    PictureUrl = x.PictureUrl,
                    Price = x.Price,
                    ProductId = x.ProductId,
                    ProductName = x.ProductName
                });
            });

            await sendEndpoint.Send(createOrderMessageCommand);
            
            //paymentDto ile ödeme işlemi gerçekleştirilebilir.
            return CreateActionResultInstance(Shared.Response<NoContent>.Success(HttpStatusCode.OK));
        }
    }
}