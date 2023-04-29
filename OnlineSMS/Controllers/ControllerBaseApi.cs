using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace OnlineSMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerBaseApi : ControllerBase
    {
        protected string GetUserId()
        {
            var jwtToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var claims = handler.ReadJwtToken(jwtToken).Claims;
            var userId = claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            return userId;
        }
    }
}
