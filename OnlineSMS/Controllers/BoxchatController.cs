using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineSMS.RequestModels;
using OnlineSMS.Services.FriendshipService;
using Microsoft.AspNetCore.Authorization;

using System.Data;
using OnlineSMS.Services.Boxchat;

namespace OnlineSMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoxchatController : ControllerBaseApi
    {

        private readonly BoxchatService boxchatService;
        public BoxchatController(BoxchatService boxchatService)
        {
            this.boxchatService = boxchatService;
        }


        [Authorize(Roles = "User")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await boxchatService.GetBoxchats(GetUserId());
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        [Route("get-messages")]
        public async Task<IActionResult> GetMessages(BoxchatMessages boxchat)
        {
            var result = await boxchatService.GetMessages(boxchat.BoxchatId,GetUserId());
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        [Route("read-all-messages")]
        public async Task<IActionResult> ReadAllMessages(BoxchatMessages boxchat)
        {
            var result = await boxchatService.SetReadAllMessasesBoxchat(GetUserId(), boxchat.BoxchatId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        
        [Authorize(Roles = "User")]
        [HttpPost]
        [Route("create-group")]
        public async Task<IActionResult> CreateGroup(CreateGroup group)
        {
            group.Members.Add(GetUserId());

            var result = await boxchatService.CreateGroup(group);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }


    }


}
