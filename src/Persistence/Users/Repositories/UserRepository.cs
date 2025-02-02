using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Users.Repositories;

public sealed class UserRepository(ApplicationDbContext dbContext) : IUserRepository
{
    public async Task<List<User>> GetUsersAsync(CancellationToken cancellationToken = default)
        => await dbContext
            .Set<User>()
            .ToListAsync(cancellationToken);

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext
            .Set<User>()
            .FirstOrDefaultAsync(member => member.Id == id, cancellationToken);

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default) =>
        await dbContext
            .Set<User>()
            .FirstOrDefaultAsync(member => member.Email == email, cancellationToken);

    public async Task<bool> IsEmailUniqueAsync(
        Email email,
        CancellationToken cancellationToken = default) =>
        !await dbContext
            .Set<User>()
            .AnyAsync(member => member.Email == email, cancellationToken);

    public void Add(User member) =>
        dbContext.Set<User>().Add(member);

    public void Update(User member) =>
        dbContext.Set<User>().Update(member);
}