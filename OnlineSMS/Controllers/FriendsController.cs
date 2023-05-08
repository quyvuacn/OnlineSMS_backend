using Microsoft.AspNetCore.Mvc;
using OnlineSMS.Services.FriendshipService;
using OnlineSMS.RequestModels;
using Microsoft.AspNet.SignalR;
using Authorization = Microsoft.AspNetCore.Authorization;

namespace OnlineSMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendsController : ControllerBaseApi
    {
        private readonly FriendshipService friendshipService;

        public FriendsController(FriendshipService friendshipService)
        {
            this.friendshipService = friendshipService;
        }


        [Authorization.Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> Index(SearchPhonenumber search)
        {
            var result = await friendshipService.FindPhoneNumber(search.PhoneNumber,GetUserId());
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorization.Authorize(Roles = "User")]
        [Route("add-friend")]
        [HttpPost]
        public async Task<IActionResult> AddFriend(AddFriend friend)
        {
            FriendRequest friendRequest = new FriendRequest
            {
                UserRequestId = GetUserId(),
                UserAcceptId = friend.UserId
            };
            var result = await friendshipService.AddFriend(friendRequest);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        
        [Authorization.Authorize(Roles = "User")]
        [Route("delete-friend-request")]
        [HttpPost]
        public async Task<IActionResult> DeleteFriendRequest(FriendRequest friendRequest)
        {
            var result = await friendshipService.DeleteFriendRequest(friendRequest);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorization.Authorize(Roles = "User")]
        [Route("agree-friend-request")]
        [HttpPost]
        public async Task<IActionResult> AgreeFriendRequest(FriendRequest friendRequest)
        {
            var result = await friendshipService.AgreeFriendRequest(friendRequest);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        
        [Authorization.Authorize(Roles = "User")]
        [Route("unfriend")]
        [HttpPost]
        public async Task<IActionResult> Unfriend(FriendRequest friendRequest)
        {
            var result = await friendshipService.Unfriend(friendRequest);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorization.Authorize(Roles = "User")]
        [Route("list-friend")]
        [HttpGet]
        public async Task<IActionResult> ListFriend()
        {
            var result = await friendshipService.ListFriend(GetUserId());
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorization.Authorize(Roles = "User")]
        [Route("list-friend-request")]
        [HttpGet]
        public async Task<IActionResult> ListFriendRequest()
        {
            var result = await friendshipService.ListFriendRequest(GetUserId());
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorization.Authorize(Roles = "User")]
        [Route("list-friend-accept")]
        [HttpGet]
        public async Task<IActionResult> ListFriendAccept()
        {
            var result = await friendshipService.ListFriendAccept(GetUserId());
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }



       
    }
}
