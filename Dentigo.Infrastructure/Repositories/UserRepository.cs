using Dentigo.Application.Interfaces.Repositories;
using Dentigo.Domain.Entities;
using Dentigo.Domain.Enums;
using Dentigo.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dentigo.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DentigoDbContext _context;

        public UserRepository(DentigoDbContext context)
        {
            _context = context;
        }

        public async Task<int?> GetActiveDoctorIdAsync(CancellationToken cancellationToken = default)
        {
            var doctorId = await _context.Users
                .Where(u => u.Role == UserRole.Doctor && u.IsActive)
                .Select(u => (int?)u.Id)
                .FirstOrDefaultAsync(cancellationToken);

            return doctorId;
        }

        public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }
    }
}
