using System.Threading.Tasks;
using FreeCourse.Web.Models;

namespace FreeCourse.Web.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<bool> RecivePayment(PaymentInfoInput paymentInfoInput);
    }
}