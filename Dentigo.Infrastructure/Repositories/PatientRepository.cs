using Dentigo.Application.Common;
using Dentigo.Application.Interfaces.Repositories;
using Dentigo.Application.Models;
using Dentigo.Domain.Entities;
using Dentigo.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dentigo.Infrastructure.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly DentigoDbContext _context;

        public PatientRepository(DentigoDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Patient>> GetPagedAsync(PatientFilterParameters parameters, CancellationToken cancellationToken = default)
        {
            IQueryable<Patient> query = _context.Patients.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(parameters.SearchString))
            {
                var search = parameters.SearchString.Trim();
                query = query.Where(p =>
                    p.FullName.Contains(search) ||
                    (p.PhoneNumber != null && p.PhoneNumber.Contains(search)));
            }

            if (parameters.CreatedFrom.HasValue)
            {
                var fromDate = parameters.CreatedFrom.Value.Date;
                query = query.Where(p => p.CreatedAt >= fromDate);
            }

            if (parameters.CreatedTo.HasValue)
            {
                var toDateExclusive = parameters.CreatedTo.Value.Date.AddDays(1);
                query = query.Where(p => p.CreatedAt < toDateExclusive);
            }

            query = parameters.SortOrder switch
            {
                "name_desc" => query.OrderByDescending(p => p.FullName).ThenBy(p => p.Id),
                "date_asc" => query.OrderBy(p => p.CreatedAt).ThenBy(p => p.Id),
                "date_desc" => query.OrderByDescending(p => p.CreatedAt).ThenByDescending(p => p.Id),
                _ => query.OrderByDescending(p => p.CreatedAt).ThenByDescending(p => p.Id)
            };

            var totalCount = await query.CountAsync(cancellationToken);

            var skip = (int)Math.Min((long)(parameters.PageNumber - 1) * parameters.PageSize, int.MaxValue);
            var items = await query
                .Skip(skip)
                .Take(parameters.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<Patient>(items, totalCount, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<Patient?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Patients.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Patients.AnyAsync(p => p.Id == id, cancellationToken);
        }

        public async Task AddAsync(Patient patient, CancellationToken cancellationToken = default)
        {
            await _context.Patients.AddAsync(patient, cancellationToken);
        }

        public void Update(Patient patient)
        {
            _context.Patients.Update(patient);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
