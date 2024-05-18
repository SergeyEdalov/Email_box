using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace CheckUnputDataLibrary
{
    public static class CheckerLibrary
    {
        public static bool CheckLengthPassword(string password)
        {
            int lengthPassword = password.Length;
            int optimalLength = 6;

            if (lengthPassword < optimalLength) { return false; }

            return true;
        }
        public static bool CheckDifficultPassword(string password)
        {
            if (password.Any(char.IsLetter)
                && password.Any(char.IsDigit)
                && password.Any(char.IsPunctuation)
                && password.Any(char.IsLower)
                && password.Any(char.IsUpper)) { return true; }
            return false;
        }
        public static bool CheckEmail(string email)
        {
            bool isValidFormat = System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

            return isValidFormat;
        }

        public static bool ValidateToken(string token, RsaSecurityKey secretKey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters(secretKey);

            try
            {
                SecurityToken validatedToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                return true;
            }
            catch (SecurityTokenException)
            {
                return false;
            }
        }

        private static TokenValidationParameters GetValidationParameters(RsaSecurityKey secretKey)
        {
            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = secretKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        }
    }
}
