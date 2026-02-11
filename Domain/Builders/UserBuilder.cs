using Visitapp.Domain.Entities;

namespace Visitapp.Domain.Builders
{
    public class UserBuilder
    {
        private string _fullName = string.Empty;
        private string _email = string.Empty;
        private string _phone = string.Empty;
        private string _passwordHash = string.Empty;
        private int? _churchId;
        private bool _isVerified = false;

        public UserBuilder WithFullName(string fullName)
        {
            _fullName = fullName?.Trim() ?? throw new ArgumentNullException(nameof(fullName));
            return this;
        }

        public UserBuilder WithEmail(string email)
        {
            _email = email?.Trim().ToLowerInvariant() ?? throw new ArgumentNullException(nameof(email));
            return this;
        }

        public UserBuilder WithPhone(string phone)
        {
            _phone = phone?.Trim() ?? throw new ArgumentNullException(nameof(phone));
            return this;
        }

        public UserBuilder WithPasswordHash(string passwordHash)
        {
            _passwordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
            return this;
        }

        public UserBuilder WithChurch(int? churchId)
        {
            _churchId = churchId;
            return this;
        }

        public UserBuilder AsVerified()
        {
            _isVerified = true;
            return this;
        }

        public UserBuilder AsUnverified()
        {
            _isVerified = false;
            return this;
        }

        public User Build()
        {
            ValidateRequiredFields();
            return User.Create(_fullName, _email, _phone, _passwordHash, _churchId);
        }

        private void ValidateRequiredFields()
        {
            if (string.IsNullOrWhiteSpace(_fullName))
                throw new InvalidOperationException("Full name is required");
            
            if (string.IsNullOrWhiteSpace(_email))
                throw new InvalidOperationException("Email is required");
            
            if (string.IsNullOrWhiteSpace(_phone))
                throw new InvalidOperationException("Phone is required");
            
            if (string.IsNullOrWhiteSpace(_passwordHash))
                throw new InvalidOperationException("Password hash is required");
        }

        public static UserBuilder New() => new();
    }
}