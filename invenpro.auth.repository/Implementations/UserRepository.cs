using invenpro.auth.domain.AggregatesModel.UserAggregate;
using invenpro.auth.domain.Seedwork;
using Microsoft.EntityFrameworkCore;

namespace invenpro.auth.repository.Implementations;

internal sealed class UserRepository(AuthServiceDbContext context) : IUserRepository
{
    private readonly AuthServiceDbContext _context = context;

    public IUnitOfWork UnitOfWork => _context;

    public async Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        User? user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return user;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        User? user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

        return user;
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        bool exists = await _context.Users
            .AsNoTracking()
            .AnyAsync(x => x.Email == email, cancellationToken);

        return exists;
    }

    public User Add(User user)
    {
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<User> entry = _context.Users.Add(user);
        return entry.Entity;
    }

    public User Update(User user)
    {
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<User> entry = _context.Users.Update(user);
        return entry.Entity;
    }
}