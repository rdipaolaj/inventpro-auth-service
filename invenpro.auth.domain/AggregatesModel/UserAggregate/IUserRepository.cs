using invenpro.auth.domain.Seedwork;

namespace invenpro.auth.domain.AggregatesModel.UserAggregate;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);

    User Add(User user);

    User Update(User user);
}