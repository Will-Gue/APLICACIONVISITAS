using Visitapp.Domain.Entities;
using Visitapp.Domain.Specifications;

namespace Visitapp.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetBySpecificationAsync(ISpecification<User> specification);
        Task<IEnumerable<User>> GetBySpecificationAsync(ISpecification<User> specification, bool asNoTracking = false);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByPhoneAsync(string phone);
        Task<User> CreateAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetWithRolesAsync(int id);
        Task<int> CountAsync(ISpecification<User> specification);
    }
}