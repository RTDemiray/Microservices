using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FreeCourse.IdentityServer.Dtos;
using FreeCourse.IdentityServer.Models;
using FreeCourse.Shared;
using FreeCourse.Shared.ControllerBases;
using FreeCourse.Shared.Dtos;
using IdentityServer4;
using IdentityServer4.Extensions;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.IdentityServer.Controllers
{
    [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : CustomBaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public UserController(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignupDto signupDto)
        {
            var user = _mapper.Map<ApplicationUser>(signupDto);

            var result = await _userManager.CreateAsync(user, signupDto.Password);

            if (!result.Succeeded)
            {
                return CreateActionResultInstance(Response<NoContent>.Fail(result.Errors.Select(x=>x.Description).ToList(),HttpStatusCode.BadRequest));
            }

            return CreateActionResultInstance(Response<NoContent>.Success(HttpStatusCode.OK));
        }

        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;
            if(string.IsNullOrEmpty(userIdClaim)) return CreateActionResultInstance(Response<NoContent>.Fail(new List<string>{"Kullanıcı bulunamadı!"},HttpStatusCode.BadRequest));

            var user = await _userManager.FindByIdAsync(userIdClaim);
            if(user == null) return CreateActionResultInstance(Response<NoContent>.Fail(new List<string>{"Kullanıcı bulunamadı!"},HttpStatusCode.BadRequest));

            var userNew = _mapper.Map<UserDto>(user);
            
            return CreateActionResultInstance(Response<UserDto>.Success(userNew, HttpStatusCode.OK));
        }
    }
}