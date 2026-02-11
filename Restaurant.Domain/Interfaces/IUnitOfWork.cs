using Restaurant.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ITableRepository Tables { get; }
    ICategoryRepository Categories { get; }
    IProductRepository Products { get; }
    IBasketItemRepository BasketItems { get; }
    IOrderRepository Orders { get; }
    IWaiterRepository Waiters { get; }
    IChatMessageRepository ChatMessages { get; }
    IPaymentRepository Payments { get; }
    IReservationRepository Reservations { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
