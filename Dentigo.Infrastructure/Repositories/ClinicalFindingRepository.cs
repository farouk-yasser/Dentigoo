using Dentigo.Application.Interfaces.Repositories;
using Dentigo.Domain.Entities;
using Dentigo.Infrastructure.DbContexts;
using System.Threading;
using System.Threading.Tasks;

namespace Dentigo.Infrastructure.Repositories
{
    public class ClinicalFindingRepository : IClinicalFindingRepository
    {
        private readonly DentigoDbContext _context;

        public ClinicalFindingRepository(DentigoDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ClinicalFinding clinicalFinding, CancellationToken cancellationToken = default)
        {
            await _context.ClinicalFindings.AddAsync(clinicalFinding, cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
