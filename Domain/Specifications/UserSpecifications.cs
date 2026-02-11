using Visitapp.Domain.Entities;

namespace Visitapp.Domain.Specifications
{
    public class UserByEmailSpecification : BaseSpecification<User>
    {
        public UserByEmailSpecification(string email) : base(u => u.Email == email.ToLowerInvariant())
        {
            AddInclude(u => u.Church!);
            AddInclude(u => u.UserRoles);
        }
    }

    public class UserWithRolesSpecification : BaseSpecification<User>
    {
        public UserWithRolesSpecification(int userId) : base(u => u.Id == userId)
        {
            AddInclude(u => u.Church!);
            AddInclude(u => u.UserRoles);
        }
    }

    public class ActiveUsersSpecification : BaseSpecification<User>
    {
        public ActiveUsersSpecification() : base(u => u.IsVerified)
        {
            AddInclude(u => u.Church!);
            ApplyOrderBy(u => u.FullName);
        }
    }
}