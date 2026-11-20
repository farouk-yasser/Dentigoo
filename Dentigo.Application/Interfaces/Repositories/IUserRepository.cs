using Dentigo.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Dentigo.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<int?> GetActiveDoctorIdAsync(CancellationToken cancellationToken = default);
        Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}
