using DocumentManagementSystem.DataAccess.Interfaces;
using DocumentManagementSystem.Entities;
using System.Threading.Tasks;

namespace DocumentManagementSystem.DataAccess.UnitOfWork
{
    public interface IUow
    {
        IRepository<T> GetRepository<T>() where T : BaseEntity;
        Task SaveChangesAsync();
    }
}
