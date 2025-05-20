using Gosuji.API.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Gosuji.API.Services
{
    public class JwtService
    {
        public const string TOKEN_COOKIE_NAME = "token";
        public const string REFRESH_TOKEN_COOKIE_NAME = "refreshToken";

        private IDbContextFactory<ApplicationDbContext> dbContextFactory;
        private IConfiguration configuration;
        private TokenValidationParameters tokenValidationParameters;

        private SigningCredentials creds;

        public JwtService(IDbContextFactory<ApplicationDbContext> dbContextFactory, IConfiguration configuration,
            TokenValidationParameters tokenValidationParameters)
        {
            this.dbContextFactory = dbContextFactory;
            this.configuration = configuration;
            this.tokenValidationParameters = tokenValidationParameters;

            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            creds = new(key, SecurityAlgorithms.HmacSha512Signature);
        }

        private async Task<string> CreateToken(User user, UserManager<User> userManager)
        {
            List<Claim> claims = [
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.Email, user.Email)];

            IList<string> roles = await userManager.GetRolesAsync(user);
            foreach (string role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            JwtSecurityToken tokenObj = new(
                issuer: configuration["BackendUrl"],
                audience: configuration["FrontendUrl"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: creds);

            string token = new JwtSecurityTokenHandler().WriteToken(tokenObj);
            return token;
        }

        private async Task<string> CreateRefreshToken(User user)
        {
            byte[] randomNumber = new byte[32];
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            string refreshToken = Convert.ToBase64String(randomNumber);

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            await dbContext.RefreshTokens.AddAsync(new()
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpireDate = DateTime.UtcNow.AddDays(7)
            });
            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();

            return refreshToken;
        }

        public async Task<string> CreateCookies(User user, UserManager<User> userManager, HttpContext httpContext)
        {
            string token = await CreateToken(user, userManager);
            string refreshToken = await CreateRefreshToken(user);

            CookieOptions options = new()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            httpContext.Response.Cookies.Append(TOKEN_COOKIE_NAME, token, options);
            httpContext.Response.Cookies.Append(REFRESH_TOKEN_COOKIE_NAME, refreshToken, options);

            return token;
        }

        public void RemoveCookies(HttpContext httpContext)
        {
            httpContext.Response.Cookies.Delete(TOKEN_COOKIE_NAME);
            httpContext.Response.Cookies.Delete(REFRESH_TOKEN_COOKIE_NAME);
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            JwtSecurityTokenHandler tokenHandler = new();
            TokenValidationParameters validationParameters = tokenValidationParameters.Clone();
            validationParameters.ValidateLifetime = false;

            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken securityToken);
            return securityToken is JwtSecurityToken jwtSecurityToken ? principal : null;
        }
    }
}
