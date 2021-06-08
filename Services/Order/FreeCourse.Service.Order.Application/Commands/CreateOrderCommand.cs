using System.Collections.Generic;
using FreeCourse.Service.Order.Application.Dtos;
using FreeCourse.Shared;
using MediatR;

namespace FreeCourse.Service.Order.Application.Commands
{
    public class CreateOrderCommand : IRequest<Response<CreatedOrderDto>>
    {
        public string BuyerId { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
        public AddressDto Address { get; set; }
    }
}