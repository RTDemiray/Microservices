using System.Threading.Tasks;

namespace FreeCourse.Web.Services.Interfaces
{
    public interface IClientCredentialTokenService
    {
        public Task<string> GetToken();
    }
}