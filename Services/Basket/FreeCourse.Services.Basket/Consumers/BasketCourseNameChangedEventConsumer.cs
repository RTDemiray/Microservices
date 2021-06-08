using System.Linq;
using System.Threading.Tasks;
using FreeCourse.Services.Basket.Services;
using FreeCourse.Shared.Messages;
using MassTransit;

namespace FreeCourse.Services.Basket.Consumers
{
    public class BasketCourseNameChangedEventConsumer : IConsumer<BasketCourseNameChangedEvent>
    {
        private readonly IBasketService _basketService;

        public BasketCourseNameChangedEventConsumer(IBasketService basketService)
        {
            _basketService = basketService;
        }

        public async Task Consume(ConsumeContext<BasketCourseNameChangedEvent> context)
        {
            var basket = await _basketService.GetBasket(context.Message.UserId);
            
            basket.Data.BasketItems.Where(x=>x.CourseId == context.Message.CourseId).ToList().ForEach(x =>
            {
                x.CourseName = context.Message.UpdatedName;
            });

            await _basketService.SaveOrUpdate(basket.Data);
        }
    }
}