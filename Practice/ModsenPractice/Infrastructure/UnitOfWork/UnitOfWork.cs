using ModsenPractice.Data;
using ModsenPractice.Patterns.Repository;
using ModsenPractice.Patterns.Repository.Interfaces;

namespace ModsenPractice.Patterns.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ModsenPracticeContext _context;
        private IUserRepository _userRepository;
        private IRoleRepository _roleRepository;

        public UnitOfWork(ModsenPracticeContext context)
        {
            _context = context;
        }

        public IUserRepository UserRepository 
        {
            get { return _userRepository ??= new UserRepository(_context); }
        }

        public IRoleRepository RoleRepository
        {
            get { return _roleRepository ??= new RoleRepository(_context); }
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}