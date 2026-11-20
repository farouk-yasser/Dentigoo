using Dentigo.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Dentigo.Application.Interfaces.Repositories
{
    public interface IClinicalFindingRepository
    {
        Task AddAsync(ClinicalFinding clinicalFinding, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
