using ModelContextProtocol.Server;
using System.ComponentModel;

namespace StreamableHttpWebApp.Tools
{
    public class UtilityTools
    {
        [McpServerTool, Description("Generates a secure random password string with a streangth.")]
        public async Task<string> GenerateRandomPassword(
            [Description("Password length")] int length = 12,
            [Description("Include numbers")] bool includeNumbers = true,
            [Description("Include special characters")] bool includeSpecialCharacters = true)
        {
            const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
            const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numberChars = "0123456789";
            const string specialChars = "!@#$%^&*()-_=+[]{}|;:,.<>?";
            string validCharacters = lowerChars + upperChars;
            if (includeNumbers)
            {
                validCharacters += numberChars;
            }
            if (includeSpecialCharacters)
            {
                validCharacters += specialChars;
            }

            var password = new List<char>();
            var randomCharacter = new Random();

            for (int i = 0; i < length; i++)
            {
                password.Add(validCharacters[randomCharacter.Next(validCharacters.Length)]);
            }
            string passwordStr = new string(password.ToArray());
            var assessment = PasswordAssessment(passwordStr);
            return $"Generated Password: {passwordStr}\nStrength: {assessment}";
        }
        private string PasswordAssessment(string password)
        {
            int score = 0;
            if (password.Length >= 12) score++;
            if (password.Any(char.IsLower)) score++;
            if (password.Any(char.IsUpper)) score++;
            if (password.Any(char.IsDigit)) score++;
            if (password.Any(c => !char.IsLetterOrDigit(c))) score++;
            return score switch
            {
                5 => "Very Strong",
                4 => "Strong",
                3 => "Moderate",
                2 => "Weak",
                _ => "Very Weak",
            };
        }
    }
}
