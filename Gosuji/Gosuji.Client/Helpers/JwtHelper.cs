using System.Security.Claims;
using System.Text.Json;

namespace Gosuji.Client.Helpers
{
    public class JwtHelper
    {
        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            List<Claim> claims = new();
            string payload = jwt.Split('.')[1];

            // ParseBase64WithoutPadding
            switch (payload.Length % 4)
            {
                case 2: payload += "=="; break;
                case 3: payload += "="; break;
            }
            byte[] jsonBytes = Convert.FromBase64String(payload);

            Dictionary<string, object>? keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            keyValuePairs.TryGetValue(ClaimTypes.Role, out object roles);

            if (roles != null)
            {
                if (roles.ToString().Trim().StartsWith("["))
                {
                    string[]? parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());

                    foreach (string parsedRole in parsedRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                    }
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
                }

                keyValuePairs.Remove(ClaimTypes.Role);
            }

            claims.AddRange(keyValuePairs
                .Where(kvp => kvp.Key is not ClaimTypes.Role and not "sub")
                .Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));

            return claims;
        }

        public static bool ClaimsEquals(IEnumerable<Claim> claims1, IEnumerable<Claim> claims2)
        {
            if (claims1 == null || claims2 == null)
            {
                return false;
            }

            if (claims1.Count() != claims2.Count())
            {
                return false;
            }

            return claims1.All(c1 => claims2.Any(c2 => c1.Type == c2.Type && c1.Value == c2.Value));
        }
    }
}
