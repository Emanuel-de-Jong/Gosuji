using Gosuji.API.Data;
using Gosuji.API.Services;
using Gosuji.Client.Data;
using Gosuji.Client.Data.Attributes;
using Gosuji.Client.Resources.Translations;
using Gosuji.Client.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Gosuji.API.Controllers.UserController
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController(IDbContextFactory<ApplicationDbContext> dbContextFactory, UserManager<User> userManager,
        JwtService jwtService) : CustomControllerBase
    {
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> CheckAuthorized()
        {
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<string>> Register([FromBody] VMRegister model)
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();

            if (await dbContext.Users.AnyAsync(u => u.NormalizedEmail == model.Email.ToUpper()))
            {
                return Accepted(APIResponses.User_Register_EmailExists);
            }

            User user = new()
            {
                UserName = model.UserName,
                Email = model.Email,
            };

            string backupCode = Guid.NewGuid().ToString().Replace("-", "");
            user.BackupCode = backupCode;

            long? languageId = (await dbContext.Languages.Where(l => l.Short == CultureInfo.CurrentCulture.TwoLetterISOLanguageName).FirstOrDefaultAsync())?.Id;
            languageId ??= 1;

            SettingConfig settingConfig = new();
            settingConfig.LanguageId = languageId.Value;
            settingConfig.IsGetChangelogEmail = model.IsGetChangelogEmail;
            await dbContext.SettingConfigs.AddAsync(settingConfig);
            await dbContext.SaveChangesAsync();

            user.SettingConfigId = settingConfig.Id;

            await dbContext.DisposeAsync();

            IdentityResult result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                StringBuilder stringBuilder = new("IdentityResult ");
                foreach (string message in result.Errors.Select(e => e.Code))
                {
                    stringBuilder.Append(message);
                    stringBuilder.Append(", ");
                }
                stringBuilder.Remove(stringBuilder.Length - 2, 2);
                return BadRequest(stringBuilder.ToString());
            }

            string userId = await userManager.GetUserIdAsync(user);

            dbContext = await dbContextFactory.CreateDbContextAsync();

            UserState userState = new();
            userState.Id = userId;
            userState.LastPresetId = (await dbContext.Presets.Where(p => p.Order == 1).FirstOrDefaultAsync()).Id;
            await dbContext.UserStates.AddAsync(userState);

            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();

            return Ok(backupCode);
        }

        [HttpPost]
        public async Task<ActionResult<string>> Login([FromBody] VMLogin model)
        {
            User? user = await userManager.FindByNameAsync(model.UserName);
            if (user == null && Regex.IsMatch(model.UserName, @"^[^@\s]+@[^@\s.]+(\.[^@\s.]+)*\.[a-zA-Z]{2,}$"))
            {
                model.UserName = (await userManager.FindByEmailAsync(model.UserName))?.UserName;
                user = await userManager.FindByNameAsync(model.UserName);
            }

            if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
            {
                return Accepted(APIResponses.User_Login_WrongCredentials);
            }

            string token = await jwtService.CreateCookies(user, userManager, HttpContext);
            return Ok(token);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Logout([FromBody] object empty)
        {
            if (empty == null)
            {
                return Forbid();
            }

            string? refreshToken = Request.Cookies[SG.RefreshTokenCookieName];
            jwtService.RemoveCookies(HttpContext);


            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest("No RefreshToken");
            }

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();

            RefreshToken? refreshTokenObj = await dbContext.RefreshTokens.Where(t => t.Token == refreshToken).FirstOrDefaultAsync();
            if (refreshTokenObj == null)
            {
                return BadRequest("Invalid RefreshToken");
            }

            if (GetUserId() != refreshTokenObj.UserId)
            {
                return Forbid();
            }

            dbContext.RefreshTokens.Remove(refreshTokenObj);

            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();

            return Ok();
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> UpdatePrivacy([FromBody] VMUpdatePrivacy model)
        {
            User user = await GetUser(userManager);
            if (user == null)
            {
                return Forbid();
            }

            if (model.UserName == user.UserName)
            {
                model.UserName = null;
            }
            if (model.Email == user.Email)
            {
                model.Email = null;
            }

            if (model.UserName == null && model.Email == null && model.NewPassword == null)
            {
                return Accepted(APIResponses.User_UpdatePrivacy_NoChanges);
            }

            if (!await userManager.CheckPasswordAsync(user, model.CurrentPassword))
            {
                return Accepted(APIResponses.User_UpdatePrivacy_WrongPassword);
            }

            string? passwordHash = null;
            if (model.NewPassword != null)
            {
                passwordHash = userManager.PasswordHasher.HashPassword(user, model.NewPassword);
            }

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            PendingUserChange? pendingUserChange = await dbContext.PendingUserChanges.Where(p => p.Id == user.Id).FirstOrDefaultAsync();
            if (pendingUserChange != null)
            {
                pendingUserChange.UserName = model.UserName ?? pendingUserChange.UserName;
                pendingUserChange.Email = model.Email ?? pendingUserChange.Email;
                pendingUserChange.Password = passwordHash ?? pendingUserChange.Password;

                dbContext.PendingUserChanges.Update(pendingUserChange);
            }
            else
            {
                pendingUserChange = new()
                {
                    Id = user.Id,
                    UserName = model.UserName,
                    Email = model.Email,
                    Password = passwordHash,
                };

                dbContext.PendingUserChanges.Add(pendingUserChange);
            }

            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();

            return Ok();
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<string>> DownloadPersonalData()
        {
            User? user = await GetUser(userManager);
            if (user == null)
            {
                return Forbid();
            }

            PersonalData personalData = new();

            IEnumerable<PropertyInfo> personalDataProps = typeof(User).GetProperties().Where(
                p => Attribute.IsDefined(p, typeof(PersonalDataAttribute)) ||
                Attribute.IsDefined(p, typeof(CustomPersonalDataAttribute)));
            foreach (PropertyInfo p in personalDataProps)
            {
                personalData.User.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
            }

            IList<UserLoginInfo> logins = await userManager.GetLoginsAsync(user);
            foreach (UserLoginInfo l in logins)
            {
                personalData.User.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
            }

            personalData.User.Add("Authenticator Key", (await userManager.GetAuthenticatorKeyAsync(user))!);

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();

            UserActivity[] userActivities = await dbContext.UserActivities.Where(a => a.UserId == user.Id).ToArrayAsync();
            personalDataProps = typeof(UserActivity).GetProperties().Where(
                p => Attribute.IsDefined(p, typeof(CustomPersonalDataAttribute)));
            foreach (UserActivity activity in userActivities)
            {
                Dictionary<string, string> instanceData = [];
                foreach (PropertyInfo p in personalDataProps)
                {
                    instanceData.Add(p.Name, p.GetValue(activity)?.ToString() ?? "null");
                }
                personalData.Activities.Add(instanceData);
            }

            Feedback[] feedbacks = await dbContext.Feedbacks.Where(f => f.UserId == user.Id).ToArrayAsync();
            personalDataProps = typeof(Feedback).GetProperties().Where(
                p => Attribute.IsDefined(p, typeof(CustomPersonalDataAttribute)));
            foreach (Feedback feedback in feedbacks)
            {
                Dictionary<string, string> instanceData = [];
                foreach (PropertyInfo p in personalDataProps)
                {
                    instanceData.Add(p.Name, p.GetValue(feedback)?.ToString() ?? "null");
                }
                personalData.Feedbacks.Add(instanceData);
            }

            await dbContext.DisposeAsync();

            byte[] fileBytes = JsonSerializer.SerializeToUtf8Bytes(personalData);

            Response.Headers.TryAdd("Content-Disposition", "attachment; filename=PersonalData.json");
            return Ok(File(fileBytes, "application/json", "PersonalData.json"));
        }

        [HttpGet]
        public async Task<ActionResult<string>> GetToken()
        {
            return !Request.Cookies.ContainsKey(SG.TokenCookieName)
                ? (ActionResult<string>)BadRequest("No token in cookie")
                : (ActionResult<string>)Ok(Request.Cookies[SG.TokenCookieName]);
        }

        [HttpGet]
        public async Task<ActionResult<string>> GetNewTokens()
        {
            User? user = await GetUser(userManager);
            if (user == null)
            {
                jwtService.RemoveCookies(HttpContext);
                return Forbid();
            }

            string? token = Request.Cookies[SG.TokenCookieName];
            if (string.IsNullOrEmpty(token))
            {
                jwtService.RemoveCookies(HttpContext);
                return BadRequest("No Token");
            }

            string? refreshToken = Request.Cookies[SG.RefreshTokenCookieName];
            if (string.IsNullOrEmpty(refreshToken))
            {
                jwtService.RemoveCookies(HttpContext);
                return BadRequest("No RefreshToken");
            }

            ClaimsPrincipal? principal = jwtService.GetPrincipalFromExpiredToken(token);
            if (principal == null)
            {
                jwtService.RemoveCookies(HttpContext);
                return BadRequest("Invalid token");
            }

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            RefreshToken? refreshTokenObj = await dbContext.RefreshTokens.Where(t => t.Token == refreshToken).FirstOrDefaultAsync();
            if (refreshTokenObj == null || refreshTokenObj.ExpireDate < DateTime.UtcNow)
            {
                jwtService.RemoveCookies(HttpContext);
                return BadRequest("Invalid refresh token");
            }

            if (refreshTokenObj.UserId != user.Id)
            {
                jwtService.RemoveCookies(HttpContext);
                return Forbid();
            }

            string newToken = await jwtService.CreateCookies(user, userManager, HttpContext);

            dbContext.RefreshTokens.Remove(refreshTokenObj);

            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();

            return Ok(newToken);
        }
    }
}
