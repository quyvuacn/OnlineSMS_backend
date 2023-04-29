using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineSMS.RequestModels;
using System.IdentityModel.Tokens.Jwt;


namespace OnlineSMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBaseApi
    {
        private readonly Services.UserService.UserService userService;

        public ProfileController(Services.UserService.UserService userService) 
        {
            this.userService = userService;
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> Index(ProfileIndex profileIndex)
        {
            var result = await userService.GetOverviewProfile(profileIndex.UserId ?? GetUserId());

            return result.IsSuccess ? Ok(result) : BadRequest(result);

        }

        [Authorize(Roles = "User")]
        [Route("check-profile")]
        [HttpGet]
        public async Task<IActionResult> CheckProfile()
        {
            var result = await userService.GetOverviewProfile(GetUserId());

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

       


        [Authorize(Roles = "User")]
        [Route("update-overview")]
        [HttpPost]
        public async Task<IActionResult> UpdateOverview(OvervieProfile overvieProfile)
        {
            var result = await userService.UpdateOverview(overvieProfile, GetUserId());

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "User")]
        [Route("more-profile")]
        [HttpGet]
        public async Task<IActionResult> GetMoreProfile()
        {
            var result = await userService.GetMoreProfile(GetUserId());

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }


        [Authorize(Roles = "User")]
        [Route("create-hobbie")]
        [HttpPost]
        public async Task<IActionResult> CreateHobbie(MoreProfileItem profileItem)
        {
            var result = await userService.CreateHobbie(GetUserId(),profileItem.Name);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "User")]
        [Route("create-cuisine")]
        [HttpPost]
        public async Task<IActionResult> CreateCuisine(MoreProfileItem profileItem)
        {
            var result = await userService.CreateCuisine(GetUserId(), profileItem.Name);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
