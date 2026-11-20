using Dentigo.Application.Interfaces.Repositories;
using Dentigo.Domain.Entities;
using Dentigo.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dentigo.Infrastructure.Repositories
{
    public class ToothRepository : IToothRepository
    {
        private readonly DentigoDbContext _context;

        public ToothRepository(DentigoDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Tooth>> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
        {
            return await _context.Teeth
                .AsNoTracking()
                .Where(t => t.PatientId == patientId)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsAsync(int patientId, string toothNumber, CancellationToken cancellationToken = default)
        {
            return await _context.Teeth
                .AnyAsync(t => t.PatientId == patientId && t.ToothNumber == toothNumber, cancellationToken);
        }

        public async Task<bool> ExistsByIdAndPatientIdAsync(int id, int patientId, CancellationToken cancellationToken = default)
        {
            return await _context.Teeth
                .AnyAsync(t => t.Id == id && t.PatientId == patientId, cancellationToken);
        }

        public async Task AddAsync(Tooth tooth, CancellationToken cancellationToken = default)
        {
            await _context.Teeth.AddAsync(tooth, cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
