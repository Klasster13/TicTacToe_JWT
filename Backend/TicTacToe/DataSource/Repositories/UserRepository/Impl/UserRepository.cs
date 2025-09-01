using Microsoft.EntityFrameworkCore;
using TicTacToe.DataSource.Context;
using TicTacToe.DataSource.Mappers;
using TicTacToe.Domain.Models;

namespace TicTacToe.DataSource.Repositories.UserRepository.Impl;

public class UserRepository(AppDbContext context) : IUserRepository
{
    private readonly AppDbContext _context = context;


    public async Task<User?> GetByIdAsync(Guid userId)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => u.ToDomainModel())
            .FirstOrDefaultAsync();
    }


    public async Task<User?> GetByLoginAsync(string login)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.Login == login)
            .Select(u => u.ToDomainModel())
            .FirstOrDefaultAsync();
    }


    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users
            .AsNoTracking()
            .OrderByDescending(u => u.UpdatedAt)
            .Select(u => u.ToDomainModel())
            .ToListAsync();
    }


    public async Task<User> AddAsync(User user)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));

        if (await IsLoginTaken(user.Login))
        {
            throw new InvalidOperationException($"Login '{user.Login}' is already taken");
        }

        var dao = user.ToDataModel();
        dao.CreatedAt = DateTime.UtcNow;
        dao.UpdatedAt = DateTime.UtcNow;

        await _context.Users.AddAsync(dao);
        await _context.SaveChangesAsync();
        return dao.ToDomainModel();
    }


    public async Task<User> UpdateAsync(User entity)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == entity.Id)
            ?? throw new KeyNotFoundException($"User {entity.Id} was not found");

        if (existingUser.Login != entity.Login && await IsLoginTaken(entity.Login))
        {
            throw new InvalidOperationException($"Login '{entity.Login}' is already taken");
        }

        existingUser.Login = entity.Login;
        existingUser.Password = entity.Password;
        existingUser.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return existingUser.ToDomainModel();
    }


    public async Task DeleteAsync(Guid userId)
    {
        var dao = await _context.Users
            .Where(u => u.Id == userId)
            .ExecuteDeleteAsync();

        if (dao == 0)
            throw new KeyNotFoundException($"User {userId} not found");
    }


    private async Task<bool> IsLoginTaken(string login)
    {
        if (string.IsNullOrWhiteSpace(login))
            throw new ArgumentException("Login cannot be empty", nameof(login));

        return await _context.Users.AnyAsync(x => x.Login == login);
    }
}
