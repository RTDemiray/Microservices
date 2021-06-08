using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FreeCourse.Services.PhotoStock.Dtos;
using FreeCourse.Shared;
using FreeCourse.Shared.ControllerBases;
using FreeCourse.Shared.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Services.PhotoStock.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotosController : CustomBaseController
    {
        [HttpPost]
        public async Task<IActionResult> Save(IFormFile photo, CancellationToken cancellationToken)
        {
            if (photo != null && photo.Length > 0)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/photos", photo.FileName);
                
                using var stream = new FileStream(path, FileMode.Create);
                await photo.CopyToAsync(stream,cancellationToken);

                var returnPath = photo.FileName;

                PhotoDto photoDto = new(){Url = returnPath};

                return CreateActionResultInstance(Response<PhotoDto>.Success(photoDto, HttpStatusCode.OK));
            }

            return CreateActionResultInstance(Response<PhotoDto>.Fail("photo is empty", HttpStatusCode.BadRequest));
        }

        [HttpDelete("{photoUrl}")]
        public IActionResult Delete(string photoUrl)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/photos", photoUrl);
            if (!System.IO.File.Exists(path))
            {
                return CreateActionResultInstance(Response<NoContent>.Fail("photo not found", HttpStatusCode.NotFound));
            }
            System.IO.File.Delete(path);
            return CreateActionResultInstance(Response<NoContent>.Success(HttpStatusCode.NoContent));
        }
    }
}