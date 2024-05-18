using System.IdentityModel.Tokens.Jwt;

namespace Message.Services
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var authorizationHeader = context.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
            {
                var token = authorizationHeader.Substring("Bearer ".Length).Trim();
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(token);

                    var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
                    if (Guid.TryParse(userIdClaim, out Guid userId))
                    {
                        context.Items["UserId"] = userId;
                    }
                }
                catch
                {
                    // Handle the exception (log it, return unauthorized response, etc.)
                }
            }

            await _next(context);
        }
    }

}
