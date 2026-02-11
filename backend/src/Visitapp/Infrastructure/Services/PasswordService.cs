using System.Text.RegularExpressions;
using Visitapp.Application.Interfaces;

namespace Visitapp.Infrastructure.Services
{
    public class PasswordService : IPasswordService
    {
        private const int MinPasswordLength = 6;
        private const int MaxPasswordLength = 100;

        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty", nameof(password));

            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
                return false;

            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch
            {
                return false;
            }
        }

        public bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (password.Length < MinPasswordLength || password.Length > MaxPasswordLength)
                return false;

            // Enhanced password validation
            var hasLower = Regex.IsMatch(password, @"[a-z]");
            var hasUpper = Regex.IsMatch(password, @"[A-Z]");
            var hasNumber = Regex.IsMatch(password, @"\d");
            var hasSpecial = Regex.IsMatch(password, @"[!@#$%^&*(),.?""':;{}|<>]");

            // At least 3 of 4 criteria for flexibility
            var criteriaCount = new[] { hasLower, hasUpper, hasNumber, hasSpecial }.Count(x => x);
            
            return criteriaCount >= 3;
        }

        public PasswordStrength GetPasswordStrength(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return PasswordStrength.VeryWeak;

            var score = 0;
            
            if (password.Length >= 8) score++;
            if (password.Length >= 12) score++;
            if (Regex.IsMatch(password, @"[a-z]")) score++;
            if (Regex.IsMatch(password, @"[A-Z]")) score++;
            if (Regex.IsMatch(password, @"\d")) score++;
            if (Regex.IsMatch(password, @"[!@#$%^&*(),.?""':;{}|<>]")) score++;

            return score switch
            {
                <= 2 => PasswordStrength.VeryWeak,
                3 => PasswordStrength.Weak,
                4 => PasswordStrength.Medium,
                5 => PasswordStrength.Strong,
                _ => PasswordStrength.VeryStrong
            };
        }
    }

    public enum PasswordStrength
    {
        VeryWeak,
        Weak,
        Medium,
        Strong,
        VeryStrong
    }
}