using ModsenPractice.Entity;

namespace ModsenPractice.Patterns.Repository.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByRefreshTokenAsync(string refreshToken);
        Task<bool> AnyAsync(string email);
        Task<IEnumerable<User>> GetUsersAsync(int page, int pageSize);
    }

}