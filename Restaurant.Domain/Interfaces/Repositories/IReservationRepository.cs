using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.Interfaces.Repositories;

public interface IReservationRepository : IGenericRepository<Reservation>
{
    Task<IReadOnlyList<Reservation>> GetReservationsByDateAsync(DateTime date);
    Task<IReadOnlyList<Reservation>> GetReservationsByTableAsync(Guid tableId);
    Task<IReadOnlyList<Reservation>> GetReservationsByStatusAsync(ReservationStatus status);
    Task<bool> IsTableReservedAsync(Guid tableId, DateTime dateTime);
}







