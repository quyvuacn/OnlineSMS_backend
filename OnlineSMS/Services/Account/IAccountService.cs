using Microsoft.AspNetCore.Identity;
using OnlineSMS.Models;
using OnlineSMS.RequestModels;

namespace OnlineSMS.Services.Account
{
    public interface IAccountService
    {
        Task<RequestResult> Register(RegisterModel model);
        Task<RequestResult> Login(LoginModel model);
        Task<RequestResult> VerifyPhoneNumber(string code);
    }

    
}
