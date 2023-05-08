
using Microsoft.AspNetCore.Mvc;
using OnlineSMS.RequestModels;
using OnlineSMS.Services.Account;

namespace OnlineSMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBaseApi
    {
        private readonly AccountService accountService;
        public AccountController(AccountService accountService)
        {
            this.accountService = accountService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {

            var result = await accountService.Login(model);
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
        
        [HttpGet]
        [Route("creare-account-test")]
        public async Task<IActionResult> CreareAccountTest()
        {
            var result = await accountService.CreareAccountTest();

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
