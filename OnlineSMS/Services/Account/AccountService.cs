using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OnlineSMS.Data;
using OnlineSMS.Lib.SendSmsLib;
using OnlineSMS.Models;
using OnlineSMS.RequestModels;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using OnlineSMS;
using Azure;

namespace OnlineSMS.Services.Account
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> userManager;
        private SignInManager<User> signInManager;
        private readonly OnlineSMSContext context;
        private readonly IConfiguration configuration;

        public AccountService(UserManager<User> userManager, OnlineSMSContext context, IConfiguration configuration, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.context = context;
            this.configuration = configuration;
            this.signInManager = signInManager;
        }

        public async Task<RequestResult> Login(LoginModel model)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber);

            bool isPersistent = true; // Lưu cookie = true
            bool lockoutOnFailure = true; //  Khóa tk nếu đăng nhập sai nhiều lần = false

            //Check  mật khẩu và lưu phiên đăng nhập -> UserLogins
            var checkIsValid = await signInManager.PasswordSignInAsync(user, model.Password, isPersistent, lockoutOnFailure);

            if (user != null && checkIsValid.Succeeded)
            {
                var userRoles = await userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.MobilePhone, user.PhoneNumber)
                };
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                //GetToken
                var token = GetToken(authClaims);

                return new RequestResult
                {
                    IsSuccess = true,
                    Message = Constants.LoginResultMessage.Success,
                    Data = new
                    {
                        phoneNumber = user.PhoneNumber, 
                        token = new JwtSecurityTokenHandler().WriteToken(token)
                    }
                };
            }
            else if (checkIsValid.IsLockedOut)
            {
                return new RequestResult
                {
                    Message = Constants.LoginResultMessage.LockedOut
                };
            }

            return new RequestResult
            {
                Message = Constants.LoginResultMessage.WrongInfo
            };
        }

        public async Task<RequestResult> Register(RegisterModel model)
        {
            var userExists = await userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber);
            var phoneNumberVerified = await context.VerificationCode.FirstOrDefaultAsync(v => v.PhoneNumber == model.PhoneNumber);

            if (userExists != null)
            {
                return new RequestResult
                {
                    Message = Constants.RegisterResultMessage.AccountExist
                };
            }
            else if(phoneNumberVerified == null || !phoneNumberVerified.Verified)
            {
                return new RequestResult
                {
                    Message = Constants.RegisterResultMessage.PhoneNumberNotVerified
                };
            }
            else if(phoneNumberVerified.ExpirationTime > DateTime.Now)
            {
                return new RequestResult
                {
                    Message = Constants.VerificationCodeMessage.CodeExpired
                };
            }

            User user = new()
            {
                PhoneNumber = model.PhoneNumber,
                UserName = model.UserName
            };

            var resultCreateUser = await userManager.CreateAsync(user, model.Password);

            if (!resultCreateUser.Succeeded)
            {
                List<string> errors = new List<string>();

                foreach (var error in resultCreateUser.Errors)
                {
                    errors.Add(error.Description.ToString());
                }
                return new RequestResult
                {
                    Message = Constants.RegisterResultMessage.BadRequest,
                    Data = new
                    {
                        errors
                    }
                };
            }

            await userManager.AddToRoleAsync(user, "User");

            return new RequestResult
            {
                IsSuccess = true,
                Message = Constants.RegisterResultMessage.Success
            };
        }

        public async Task<RequestResult> VerifyPhoneNumber(string phoneNumber)
        {
            var verificationCode = await context.VerificationCode.Where(pn => pn.PhoneNumber == phoneNumber).FirstOrDefaultAsync();

            SMS sms = new SMS(configuration);
            bool isSendSMS = false;

            if (verificationCode == null)
            {
                verificationCode = new VerificationCode(phoneNumber);
                await context.VerificationCode.AddAsync(verificationCode);
                await context.SaveChangesAsync();

                isSendSMS = sms.SendTo(phoneNumber, verificationCode.Code);
            }
            else if (!verificationCode.PhoneLockoutEnabled || verificationCode.phoneUnlockout())
            {
                verificationCode.newVerificationCode();
                await context.SaveChangesAsync();

                isSendSMS = sms.SendTo(phoneNumber, verificationCode.Code);
            }

            return isSendSMS ? new RequestResult
            {
                IsSuccess = true,
                Message = Constants.VerificationCodeMessage.Success
            } : new RequestResult
            {
                Message = Constants.VerificationCodeMessage.PhoneLockedOut
            };
        }


        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));
            var token = new JwtSecurityToken(
                issuer: configuration["JWT:ValidIssuer"],
                audience: configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
            return token;
        }
    }

    
}
