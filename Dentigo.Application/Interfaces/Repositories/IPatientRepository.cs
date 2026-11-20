using Dentigo.Application.Common;
using Dentigo.Application.Models;
using Dentigo.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Dentigo.Application.Interfaces.Repositories
{
    public interface IPatientRepository
    {
        Task<PagedResult<Patient>> GetPagedAsync(PatientFilterParameters parameters, CancellationToken cancellationToken = default);
        Task<Patient?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
        Task AddAsync(Patient patient, CancellationToken cancellationToken = default);
        void Update(Patient patient);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
