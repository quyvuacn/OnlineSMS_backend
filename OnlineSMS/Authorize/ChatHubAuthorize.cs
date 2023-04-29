using Azure.Core;
using Microsoft.AspNetCore.Mvc.Filters;
using NuGet.Configuration;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace OnlineSMS.Authorize
{
    public class ChatHubAuthorize : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string jwtToken = context.HttpContext.Request.Cookies["token"].ToString()
                ?? context.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var handler = new JwtSecurityTokenHandler();
            var claims = handler.ReadJwtToken(jwtToken).Claims;

            var userId = claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var roles = claims.ToList();

        }
    }
}
