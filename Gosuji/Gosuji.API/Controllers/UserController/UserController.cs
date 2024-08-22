using Gosuji.API.Data;
using Gosuji.API.Services;
using Gosuji.Client.Data;
using Gosuji.Client.Data.Attributes;
using Gosuji.Client.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private string IdentityResultToString(IdentityResult result)
        {
            StringBuilder stringBuilder = new("IdentityResult ");
            foreach (string message in result.Errors.Select(e => e.Code))
            {
                stringBuilder.Append(message);
                stringBuilder.Append(", ");
            }
            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            return stringBuilder.ToString();
        }

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
                return Accepted("", "User_Register_EmailExists");
            }

            User user = new()
            {
                UserName = model.UserName,
                Email = model.Email,
            };

            string backupCode = Guid.NewGuid().ToString().Replace("-", "");
            user.BackupCode = backupCode;

            SettingConfig settingConfig = new();
            settingConfig.LanguageId = model.Language;
            settingConfig.IsGetChangelogEmail = model.IsGetChangelogEmail;
            await dbContext.SettingConfigs.AddAsync(settingConfig);
            await dbContext.SaveChangesAsync();

            user.SettingConfigId = settingConfig.Id;

            await dbContext.DisposeAsync();

            IdentityResult result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(IdentityResultToString(result));
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
                user = await userManager.FindByEmailAsync(model.UserName);
            }

            if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
            {
                return Accepted("", "User_Login_WrongCredentials");
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

            string? refreshToken = Request.Cookies[JwtService.REFRESH_TOKEN_COOKIE_NAME];
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
        public async Task<ActionResult> ChangeEmail([FromBody] VMChangeEmail model)
        {
            User? user = await GetUser(userManager);
            if (user == null)
            {
                if (model.UserName == null || model.Password == null)
                {
                    return Accepted("", "User_Login_WrongCredentials");
                }

                user = await userManager.FindByNameAsync(model.UserName);
                if (user == null && Regex.IsMatch(model.UserName, @"^[^@\s]+@[^@\s.]+(\.[^@\s.]+)*\.[a-zA-Z]{2,}$"))
                {
                    user = await userManager.FindByEmailAsync(model.UserName);
                }

                if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
                {
                    return Accepted("", "User_Login_WrongCredentials");
                }
            }

            if (user.BackupCode != model.BackupCode)
            {
                return Accepted("", "User_ChangeEmail_WrongBackupCode");
            }

            IdentityResult result = await userManager.SetEmailAsync(user, model.NewEmail);
            if (!result.Succeeded)
            {
                return BadRequest(IdentityResultToString(result));
            }

            await LogOutEverywhere(user);

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> ForgotPassword([FromBody] VMForgotPassword model)
        {
            User? user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Accepted("", "User_ForgotPassword_EmailNotFound");
            }

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> ChangePassword([FromBody] VMChangePassword model)
        {
            return Ok();

            User user = null; // TODO: Get from email token
            IdentityResult result = await userManager.RemovePasswordAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(IdentityResultToString(result));
            }

            result = await userManager.AddPasswordAsync(user, model.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(IdentityResultToString(result));
            }

            await LogOutEverywhere(user);

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
                return Accepted("", "User_UpdatePrivacy_NoChanges");
            }

            if (!await userManager.CheckPasswordAsync(user, model.CurrentPassword))
            {
                return Accepted("", "User_UpdatePrivacy_WrongPassword");
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

        private async Task LogOutEverywhere(User user)
        {
            jwtService.RemoveCookies(HttpContext);

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();

            await dbContext.RefreshTokens
                .Where(t => t.UserId == user.Id)
                .ForEachAsync(t => dbContext.RefreshTokens.Remove(t));

            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<byte[]>> DownloadPersonalData([FromBody] object empty)
        {
            if (empty == null)
            {
                return Forbid();
            }

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
            return Ok(fileBytes);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> DeletePersonalData(VMDeletePersonalData model)
        {
            User? user = await GetUser(userManager);
            if (user == null)
            {
                return Forbid();
            }

            if (!await userManager.CheckPasswordAsync(user, model.Password))
            {
                return Accepted("", "User_DeletePersonalData_WrongPassword");
            }

            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<string>> GetToken()
        {
            return !Request.Cookies.ContainsKey(JwtService.TOKEN_COOKIE_NAME)
                ? (ActionResult<string>)BadRequest("No token in cookie")
                : (ActionResult<string>)Ok(Request.Cookies[JwtService.TOKEN_COOKIE_NAME]);
        }

        [HttpPost]
        public async Task<ActionResult<string>> GetNewTokens([FromBody] object empty)
        {
            if (empty == null)
            {
                return Forbid();
            }

            string? token = Request.Cookies[JwtService.TOKEN_COOKIE_NAME];
            if (string.IsNullOrEmpty(token))
            {
                return GetNewTokensError(BadRequest("No token"));
            }

            string? refreshToken = Request.Cookies[JwtService.REFRESH_TOKEN_COOKIE_NAME];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return GetNewTokensError(BadRequest("No refreshToken"));
            }

            ClaimsPrincipal? principal = jwtService.GetPrincipalFromExpiredToken(token);
            if (principal == null)
            {
                return GetNewTokensError(BadRequest("Invalid token"));
            }

            string? userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return GetNewTokensError(BadRequest("No user id in token"));
            }

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            RefreshToken? refreshTokenObj = await dbContext.RefreshTokens.Where(t => t.Token == refreshToken).FirstOrDefaultAsync();
            if (refreshTokenObj == null || refreshTokenObj.ExpireDate < DateTime.UtcNow)
            {
                return GetNewTokensError(BadRequest("Invalid refresh token"));
            }

            if (refreshTokenObj.UserId != userId)
            {
                return GetNewTokensError(Forbid());
            }

            User? user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return GetNewTokensError(Forbid());
            }

            string newToken = await jwtService.CreateCookies(user, userManager, HttpContext);

            dbContext.RefreshTokens.Remove(refreshTokenObj);

            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();

            return Ok(newToken);
        }

        private ActionResult<string> GetNewTokensError(ActionResult<string> result)
        {
            jwtService.RemoveCookies(HttpContext);
            return result;
        }
    }
}
