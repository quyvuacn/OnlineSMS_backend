using Microsoft.AspNetCore.Mvc;
using OnlineSMS.Services.ChatHub;
using Microsoft.AspNetCore.Authorization;


namespace OnlineSMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatHubController : ControllerBaseApi
    {
        private readonly ChatHubService chatHubService;

        public ChatHubController(ChatHubService chatHubService)
        {
            this.chatHubService = chatHubService;
        }


        //[Authorize(Roles = "User")]
        [Route("connect-to-chathub")]
        [HttpGet]
        public async Task<IActionResult> ConnectToChatHub()
        {
            var result = await chatHubService.ConnectToChatHub(GetUserId());
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
