using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using FreeCourse.Services.Basket.Dtos;
using FreeCourse.Shared;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace FreeCourse.Services.Basket.Services
{
    public class BasketService : IBasketService
    {
        private readonly IRedisCacheClient _redisCacheClient;

        public BasketService(IRedisCacheClient redisCacheClient)
        {
            _redisCacheClient = redisCacheClient;
        }

        public async Task<Response<BasketDto>> GetBasket(string userId)
        {
            var basket = await _redisCacheClient.GetDbFromConfiguration().GetAsync<BasketDto>(userId);
            if (basket == null)
            {
                return Response<BasketDto>.Fail("Basket Not Found",HttpStatusCode.NotFound);
            }
            
            return Response<BasketDto>.Success(basket,HttpStatusCode.OK);
        }

        public async Task<Response<bool>> SaveOrUpdate(BasketDto basketDto)
        {
            var status = await _redisCacheClient.GetDbFromConfiguration().AddAsync(basketDto.UserId,basketDto);
            
            return status
                ? Response<bool>.Success(HttpStatusCode.NoContent)
                : Response<bool>.Fail("Basket could not update or save", HttpStatusCode.InternalServerError);
        }

        public async Task<Response<bool>> Delete(string userId)
        {
            var status = await _redisCacheClient.GetDbFromConfiguration().RemoveAsync(userId);
            
            return status
                ? Response<bool>.Success(HttpStatusCode.NoContent)
                : Response<bool>.Fail("Basket not found", HttpStatusCode.InternalServerError);
        }
    }
}