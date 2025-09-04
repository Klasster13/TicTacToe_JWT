using Microsoft.EntityFrameworkCore;
using TicTacToe.DataSource.Context;
using TicTacToe.DataSource.Mappers;
using TicTacToe.Domain.Models;

namespace TicTacToe.DataSource.Repositories.SessionRepository.Impl;

public class SessionRepository(AppDbContext context) : ISessionRepository
{
    private readonly AppDbContext _context = context;


    public async Task<Session?> GetByIdAsync(Guid id)
    {
        return await _context.Sessions
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => s.ToDomainModel())
            .FirstOrDefaultAsync();
    }


    //public async Task<IEnumerable<Session>> GetAllAsync()
    //{
    //    return await _context.Sessions
    //        .AsNoTracking()
    //        .OrderByDescending(s => s.UpdatedAt)
    //        .Select(s => s.ToDomainModel())
    //        .ToListAsync();
    //}


    public async Task AddAsync(Session entity)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        var dao = entity.ToDataModel();
        dao.CreatedAt = DateTime.UtcNow;
        dao.UpdatedAt = DateTime.UtcNow;

        await _context.Sessions.AddAsync(dao);
        await _context.SaveChangesAsync();
    }


    public async Task UpdateAsync(Session entity)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        var existingDAO = await _context.Sessions.FirstOrDefaultAsync(s => s.Id == entity.Id)
            ?? throw new KeyNotFoundException($"Session {entity.Id} was not found");

        existingDAO.Field = entity.Field.ToDataModel();
        existingDAO.State = entity.State;
        existingDAO.Mode = entity.Mode;
        existingDAO.Player1Id = entity.Player1Id;
        existingDAO.Player2Id = entity.Player2Id;
        existingDAO.WinningCells = entity.WinningCells?.CellsToDataModel();
        existingDAO.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }


    public async Task DeleteAsync(Guid id)
    {
        var dao = await _context.Sessions
            .Where(s => s.Id == id)
            .ExecuteDeleteAsync();

        if (dao == 0)
            throw new KeyNotFoundException($"Session {id} not found");
    }


    public async Task<IEnumerable<Session>> GetAvailableSessionsAsync(Guid userId)
    {
        return await _context.Sessions
            .AsNoTracking()
            .Where(s => (s.Player1Id == null || s.Player2Id == null)
                        && s.Player1Id != userId
                        && s.Player2Id != userId
                        && s.State == Common.Enums.State.WaitingForPlayers)
            .OrderByDescending(s => s.UpdatedAt)
            .Select(s => s.ToDomainModel())
            .ToListAsync();
    }


    public async Task<IEnumerable<Session>> GetUserSessionsAsync(Guid userId)
    {
        return await _context.Sessions
            .AsNoTracking()
            .Where(s => s.Player1Id == userId || s.Player2Id == userId)
            .OrderByDescending(s => s.UpdatedAt)
            .Select(s => s.ToDomainModel())
            .ToListAsync();
    }


    public async Task<IEnumerable<Session>> GetFinishedSessions(Guid userId)
    {
        return await _context.Sessions
            .AsNoTracking()
            .Where(s => (s.Player1Id == userId || s.Player2Id == userId)
                        && (s.State == Common.Enums.State.Player1Winner
                        || s.State == Common.Enums.State.Player2Winner
                        || s.State == Common.Enums.State.Draw))
            .OrderByDescending(s => s.UpdatedAt)
            .Select(s => s.ToDomainModel())
            .ToListAsync();
    }
}
