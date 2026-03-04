using Microsoft.EntityFrameworkCore;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using Restaurant.Domain.Interfaces.Repositories;
using Restaurant.Persistence.Context;

namespace Restaurant.Persistence.Repositories;

public class ReservationRepository : GenericRepository<Reservation>, IReservationRepository
{
    public ReservationRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Reservation>> GetReservationsByDateAsync(DateTime date)
    {
        return await _dbSet
            .Include(x => x.Table)
            .Where(x => x.ReservationDate.Date == date.Date)
            .OrderBy(x => x.ReservationDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public new async Task<IReadOnlyList<Reservation>> GetAllAsync()
    {
        return await _dbSet
            .Include(x => x.Table)
            .OrderByDescending(x => x.ReservationDate)
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task<IReadOnlyList<Reservation>> GetReservationsByTableAsync(Guid tableId)
    {
        return await _dbSet
            .Include(x => x.Table)
            .Where(x => x.TableId == tableId)
            .OrderByDescending(x => x.ReservationDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Reservation>> GetReservationsByStatusAsync(ReservationStatus status)
    {
        return await _dbSet
            .Include(x => x.Table)
            .Where(x => x.Status == status)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> IsTableReservedAsync(Guid tableId, DateTime dateTime)
    {
        return await _dbSet.AnyAsync(x =>
            x.TableId == tableId
            && x.ReservationDate.Date == dateTime.Date
            && x.Status != ReservationStatus.Cancelled);
    }
}