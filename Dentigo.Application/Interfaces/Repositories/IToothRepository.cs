using Dentigo.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dentigo.Application.Interfaces.Repositories
{
    public interface IToothRepository
    {
        Task<IReadOnlyList<Tooth>> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int patientId, string toothNumber, CancellationToken cancellationToken = default);
        Task<bool> ExistsByIdAndPatientIdAsync(int id, int patientId, CancellationToken cancellationToken = default);
        Task AddAsync(Tooth tooth, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
