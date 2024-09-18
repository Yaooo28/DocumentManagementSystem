using DocumentManagementSystem.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DocumentManagementSystem.Business.Interfaces
{
    public interface IAnnouncementService
    {
        Task<IEnumerable<Announcement>> GetAllAsync();
        Task<Announcement> GetByIdAsync(int id);
        Task AddAsync(Announcement announcement);
        Task UpdateAsync(Announcement announcement);
        Task DeleteAsync(int id);
    }
}