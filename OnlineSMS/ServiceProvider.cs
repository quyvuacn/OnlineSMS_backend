using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OnlineSMS.Data;
using OnlineSMS.Models;
using OnlineSMS.Services.Account;
using OnlineSMS.Services.FriendshipService;
using System.Text;
using OnlineSMS.Services.ChatHub;
using OnlineSMS.Services.Boxchat;
using Microsoft.AspNetCore.Hosting;
using OnlineSMS.Services.UploadFile;

namespace OnlineSMS
{
    public static class ServiceProvider
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add and config Identity
            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false; // Xác minh email = false
            })
                .AddEntityFrameworkStores<OnlineSMSContext>()
                .AddDefaultTokenProviders();

            // Add Jwt
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true; // Save the token in the HttpContext
                                          //options.RequireHttpsMetadata = false; // Disable HTTPS metadata requirement
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = configuration["JWT:ValidAudience"],
                    ValidIssuer = configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
                };
            });


            //Add Logic Services
            services.AddScoped<UploadFileService>();
            services.AddScoped<AccountService>();
            services.AddScoped<Services.UserService.UserService>();
            services.AddScoped<FriendshipService>();
            
            services.AddScoped<ChatHubService>();
            services.AddScoped<BoxchatService>();

            return services;
        }
    }
}
