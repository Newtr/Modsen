using Microsoft.EntityFrameworkCore;
using ModsenPractice.Patterns.Repository.Interfaces;
using ModsenPractice.Data;
using ModsenPractice.Entity;

namespace ModsenPractice.Patterns.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ModsenPracticeContext _context;

        public UserRepository(ModsenPracticeContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.Include(u => u.Role).ToListAsync();
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users.Include(u => u.Role).SingleOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.Include(u => u.Role).SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _context.Users
                .Include(u => u.Role)
                .SingleOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }

        public async Task<IEnumerable<User>> GetUsersAsync(int page, int pageSize)
        {
            return await _context.Users
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<bool> AnyAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task AddAsync(User entity)
        {
            await _context.Users.AddAsync(entity);
        }

        public async Task UpdateAsync(User entity)
        {
            _context.Users.Update(entity);
        }

        public async Task DeleteAsync(User entity)
        {
            _context.Users.Remove(entity);
        }
    }
}
