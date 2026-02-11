using Visitapp.Domain.Entities;

namespace Visitapp.Domain.Interfaces
{
    public interface IContactRepository
    {
        Task<Contact?> GetByIdAsync(int id);
        Task<Contact> CreateAsync(Contact contact);
        Task UpdateAsync(Contact contact);
        Task DeleteAsync(int id);
        Task<IEnumerable<Contact>> GetByUserIdAsync(int userId);
        Task<bool> ExistsAsync(int id);
        Task<Contact?> GetByIdAndUserIdAsync(int id, int userId);
    }
}