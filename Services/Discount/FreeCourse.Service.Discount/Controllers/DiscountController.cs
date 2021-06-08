using System.Threading.Tasks;
using FreeCourse.Service.Discount.Models;
using FreeCourse.Service.Discount.Services;
using FreeCourse.Shared.ControllerBases;
using FreeCourse.Shared.Service;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Service.Discount.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : CustomBaseController
    {
        private readonly IDiscountService _discountService;
        private readonly ISharedIdentityService _sharedIdentityService;

        public DiscountController(IDiscountService discountService, ISharedIdentityService sharedIdentityService)
        {
            _discountService = discountService;
            _sharedIdentityService = sharedIdentityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return CreateActionResultInstance(await _discountService.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return CreateActionResultInstance(await _discountService.GetById(id));
        }

        [HttpGet("GetByCodeAndUserId/{code}")]
        public async Task<IActionResult> GetByCodeAndUserId(string code)
        {
            return CreateActionResultInstance(
                await _discountService.GetByCodeAndUserId(code, _sharedIdentityService.GetUserId));
        }

        [HttpPost]
        public async Task<IActionResult> Save(Discounts discounts)
        {
            discounts.UserId = _sharedIdentityService.GetUserId;
            return CreateActionResultInstance(await _discountService.Save(discounts));
        }
        
        [HttpPut]
        public async Task<IActionResult> Update(Discounts discounts)
        {
            discounts.UserId = _sharedIdentityService.GetUserId;
            return CreateActionResultInstance(await _discountService.Update(discounts));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return CreateActionResultInstance(await _discountService.Delete(id));
        }
    }
}