using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FreeCourse.Service.Order.Application.Commands;
using FreeCourse.Service.Order.Application.Dtos;
using FreeCourse.Service.Order.Domain.OrderAggregate;
using FreeCourse.Service.Order.Infrastructure;
using FreeCourse.Shared;
using MapsterMapper;
using MediatR;

namespace FreeCourse.Service.Order.Application.Handlers
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand,Response<CreatedOrderDto>>
    {
        private readonly OrderDbContext _context;
        
        public CreateOrderCommandHandler(OrderDbContext context)
        {
            _context = context;
        }
        
        public async Task<Response<CreatedOrderDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var newAddress = new Address(request.Address.Province, request.Address.District, request.Address.Street, request.Address.ZipCode, request.Address.Line);

            Domain.OrderAggregate.Order newOrder = new(request.BuyerId, newAddress);
            
            request.OrderItems.ForEach(x =>
            {
                newOrder.AddOrderItem(x.ProductId,x.ProductName,x.PictureUrl,x.Price);
            });

            await _context.Orders.AddAsync(newOrder);
            await _context.SaveChangesAsync();

            return Response<CreatedOrderDto>.Success(new CreatedOrderDto() {OrderId = newOrder.Id},HttpStatusCode.Created);
        }
    }
}