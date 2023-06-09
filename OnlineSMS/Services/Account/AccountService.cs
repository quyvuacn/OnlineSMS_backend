﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OnlineSMS.Data;
using OnlineSMS.Lib.SendSmsLib;
using OnlineSMS.RequestModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using OnlineSMS.Models;
using OnlineSMS.Services.ChatHub;

namespace OnlineSMS.Services.Account
{
    public class AccountService : IAccountService
    {
        
        private readonly UserManager<User> userManager;
        private SignInManager<User> signInManager;
        private readonly OnlineSMSContext context;
        private readonly IConfiguration configuration;
        private readonly ChatHubService chatHubService;


        public AccountService(UserManager<User> userManager, OnlineSMSContext context, IConfiguration configuration, SignInManager<User> signInManager,ChatHubService chatHubService)
        {
            this.userManager = userManager;
            this.context = context;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.chatHubService = chatHubService;
        }

        public async Task<RequestResult> Login(LoginModel model)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber);

            bool isPersistent = true; // Lưu cookie = true
            bool lockoutOnFailure = true; //  Khóa tk nếu đăng nhập sai nhiều lần = false

            

            if (user != null)
            {
                //Check  mật khẩu và lưu phiên đăng nhập -> UserLogins
                var checkIsValid = await signInManager.PasswordSignInAsync(user, model.Password, isPersistent, lockoutOnFailure);
                var userRoles = await userManager.GetRolesAsync(user);

                if (checkIsValid.Succeeded)
                {
                    var authClaims = new List<Claim>
                {
                    new Claim("UserName", user.UserName),
                    new Claim("PhoneNumber", user.PhoneNumber),
                    new Claim("Email",user.Email),
                    new Claim("UserId",user.Id)
                };
                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }
                    //GetToken
                    var token = GetToken(authClaims);

                    var result = await chatHubService.ConnectToChatHub(user.Id);

                    var connectionId = result.Data;

                    Console.WriteLine(connectionId);

                    return new RequestResult
                    {
                        IsSuccess = true,
                        Message = Constants.LoginResultMessage.Success,
                        Data = new
                        {
                            phoneNumber = user.PhoneNumber,
                            userName = user.UserName,
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            userId = user.Id,
                            connectionId
                        }
                    };
                }
                else if (checkIsValid.IsLockedOut)
                {
                    return new RequestResult
                    {
                        Message = Constants.LoginResultMessage.LockedOut,
                    };
                }
               
            }

            return new RequestResult
            {
                Message = Constants.LoginResultMessage.WrongInfo
            };
        }

        public async Task<RequestResult> Register(RegisterModel model)
        {
            var userPhoneExists = await userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber);
            var userEmailExists = await userManager.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            var phoneNumberVerified = await context.VerificationCode.FirstOrDefaultAsync(v => v.PhoneNumber == model.PhoneNumber);

            if (phoneNumberVerified == null)
            {
                return new RequestResult
                {
                    Message = Constants.RegisterResultMessage.PhoneNumberNotVerified,
                    Data = new
                    {
                        Message = Constants.RegisterResultMessage.PhoneNumberNotVerified,
                        Field = "Verify Code"
                    }
                };
            }
            if (model.VerifyCode == phoneNumberVerified.Code)
            {
                phoneNumberVerified.UsedTime = DateTime.UtcNow;
            }

            if (userPhoneExists != null)
            {
                return new RequestResult
                {
                    Message = Constants.RegisterResultMessage.PhonenumberExist
                };
            }
            else if (userEmailExists != null)
            {
                return new RequestResult
                {
                    Message = Constants.RegisterResultMessage.EmailExist
                };
            }
            else if (model.VerifyCode != phoneNumberVerified.Code)
            {
                
                return new RequestResult
                {
                    Message = Constants.RegisterResultMessage.PhoneNumberNotVerified
                };
            }
            else if (DateTime.Now > phoneNumberVerified.ExpirationTime)
            {
                return new RequestResult
                {
                    Message = Constants.VerificationCodeMessage.CodeExpired
                };
            }
            else if (phoneNumberVerified.UsedTime == null)
            {
                return new RequestResult
                {
                    Message = Constants.VerificationCodeMessage.CodeUsed
                };
            }
            
            User user = new()
            {
                PhoneNumber = model.PhoneNumber,
                UserName = model.UserName,
                Email = model.Email
            };

            var resultCreateUser = await userManager.CreateAsync(user, model.Password);

            if (!resultCreateUser.Succeeded)
            {
                return new RequestResult
                {
                    Message = Constants.RegisterResultMessage.BadRequest,
                };
            }

            await userManager.AddToRoleAsync(user, "User");
            await context.SaveChangesAsync();

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

        public async Task<RequestResult> CreareAccountTest()
        {
            if (!userManager.Users.Any())
            {
                User user = new()
                {
                    PhoneNumber = "+84326459773",
                    UserName = "vuvietquy",
                    Email = "vuvietquyacn@gmail.com"
                };

                await userManager.CreateAsync(user, "Acn@2003");
                await userManager.AddToRoleAsync(user, "User");

                User user2 = new()
                {
                    PhoneNumber = "+84392156772",
                    UserName = "vuvietson",
                    Email = "vuvietson@gmail.com"
                };

                await userManager.CreateAsync(user2, "Acn@2003");
                await userManager.AddToRoleAsync(user2, "User");

                User user3 = new()
                {
                    PhoneNumber = "+84969264559",
                    UserName = "lamthibe",
                    Email = "lamthibe@gmail.com"
                };

                await userManager.CreateAsync(user3, "Acn@2003");
                await userManager.AddToRoleAsync(user3, "User");
            }
            
            return new RequestResult
            {
                IsSuccess = true
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
