using System.Threading.Tasks;
using FreeCourse.Service.Order.Domain.OrderAggregate;
using FreeCourse.Service.Order.Infrastructure;
using FreeCourse.Shared.Messages;
using MassTransit;

namespace FreeCourse.Service.Order.Application.Consumers
{
    public class CreateOrderMessageCommandConsumer : IConsumer<CreateOrderMessageCommand>
    {
        private readonly OrderDbContext _orderDbContext;

        public CreateOrderMessageCommandConsumer(OrderDbContext orderDbContext)
        {
            _orderDbContext = orderDbContext;
        }

        public async Task Consume(ConsumeContext<CreateOrderMessageCommand> context)
        {
            var newAddress = new Address(context.Message.Province, context.Message.District, context.Message.Street,
                context.Message.ZipCode, context.Message.Line);

            Domain.OrderAggregate.Order order = new(context.Message.BuyerId, newAddress);
            
            context.Message.OrderItems.ForEach(x =>
            {
                order.AddOrderItem(x.ProductId,x.ProductName,x.PictureUrl,x.Price);
            });

            await _orderDbContext.AddAsync(order);
            await _orderDbContext.SaveChangesAsync();
        }
    }
}