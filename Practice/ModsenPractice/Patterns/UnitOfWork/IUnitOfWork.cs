using ModsenPractice.Patterns.Repository.Interfaces;

namespace ModsenPractice.Patterns.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        IRoleRepository RoleRepository { get; }

        Task<int> CommitAsync(); // Сохраняет изменения всех репозиториев сразу
    }
}