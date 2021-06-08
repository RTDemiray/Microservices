using System.Collections.Generic;
using FreeCourse.Service.Order.Application.Dtos;
using FreeCourse.Shared;
using MediatR;

namespace FreeCourse.Service.Order.Application.Queries
{
    public class GetOrdersByUserIdQuery:IRequest<Response<List<OrderDto>>>
    {
        public string UserId { get; set; }
    }
}