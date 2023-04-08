using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OnlineSMS.Data;
using OnlineSMS.Models;
using OnlineSMS.Services.Account;
using System.Text;

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
            services.AddScoped<AccountService>();

            return services;
        }
    }
}
