using System.Collections.Generic;
using System.Threading.Tasks;
using FreeCourse.Service.Discount.Models;
using FreeCourse.Shared;
using FreeCourse.Shared.Dtos;

namespace FreeCourse.Service.Discount.Services
{
    public interface IDiscountService
    {
        Task<Response<List<Discounts>>> GetAll();

        Task<Response<Discounts>> GetById(int id);

        Task<Response<NoContent>> Save(Discounts Discounts);
        
        Task<Response<NoContent>> Update(Discounts Discounts);
        
        Task<Response<NoContent>> Delete(int id);

        Task<Response<Discounts>> GetByCodeAndUserId(string code, string userId);
    }
}