
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using OnlineSMS.RequestModels;
using OnlineSMS.Services.Account;
using Polly;

namespace OnlineSMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private AccountService accountService;

        public AccountController(AccountService accountService)
        {
            this.accountService = accountService;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return Ok(new { name = "Vũ Viết Quý" });
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {

            var result = await accountService.Login(model);
            if (result.IsSuccess)
            {
                CookieOptions options = new CookieOptions();
                options.Expires = DateTime.Now.AddDays(30);
                Response.Cookies.Append("token","vuvietquyacn", options);
            }
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var result = await accountService.Register(model);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            string authHeader = Request.Headers["Authorization"];

            if (!string.IsNullOrEmpty(authHeader))
            {
                Response.Cookies.Delete("token");
            }

            return !string.IsNullOrEmpty(authHeader) ? Ok() : BadRequest();
        }

        [HttpPost]
        [Route("verify-phonenumber")]
        public async Task<IActionResult> VerifyPhonenumber(RequestVerifyPhone requestVerifyPhone)
        {
            string phoneNumber = requestVerifyPhone.PhoneNumber;

            var result = await accountService.VerifyPhoneNumber(phoneNumber);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
