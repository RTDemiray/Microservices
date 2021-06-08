using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FreeCourse.Service.Order.Application.Dtos;
using FreeCourse.Service.Order.Application.Queries;
using FreeCourse.Service.Order.Infrastructure;
using FreeCourse.Shared;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FreeCourse.Service.Order.Application.Handlers
{
    public class GetOrdersByUserIdQueryHandler:IRequestHandler<GetOrdersByUserIdQuery,Response<List<OrderDto>>>
    {
        private readonly OrderDbContext _context;
        private readonly IMapper _mapper;

        public GetOrdersByUserIdQueryHandler(OrderDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Response<List<OrderDto>>> Handle(GetOrdersByUserIdQuery request, CancellationToken cancellationToken)
        {
            var orders = await _context.Orders.Include(x => x.OrderItems).Where(x => x.BuyerId == request.UserId)
                .ToListAsync();

            return !orders.Any()
                ? Response<List<OrderDto>>.Success(new List<OrderDto>(), HttpStatusCode.OK)
                : Response<List<OrderDto>>.Success(_mapper.Map<List<OrderDto>>(orders), HttpStatusCode.OK);
        }
    }
}