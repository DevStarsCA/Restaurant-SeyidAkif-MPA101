using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Restaurant.Domain.Interfaces;
using Restaurant.Domain.Interfaces.Repositories;
using Restaurant.Persistence.Context;
using Restaurant.Persistence.Repositories;

namespace Restaurant.Persistence.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    private ITableRepository? _tables;
    private ICategoryRepository? _categories;
    private IProductRepository? _products;
    private IBasketItemRepository? _basketItems;
    private IOrderRepository? _orders;
    private IWaiterRepository? _waiters;
    private IChatMessageRepository? _chatMessages;
    private IPaymentRepository? _payments;
    private IReservationRepository? _reservations;

    public ITableRepository Tables => _tables ??= new TableRepository(_context);
    public ICategoryRepository Categories => _categories ??= new CategoryRepository(_context);
    public IProductRepository Products => _products ??= new ProductRepository(_context);
    public IBasketItemRepository BasketItems => _basketItems ??= new BasketItemRepository(_context);
    public IOrderRepository Orders => _orders ??= new OrderRepository(_context);
    public IWaiterRepository Waiters => _waiters ??= new WaiterRepository(_context);
    public IChatMessageRepository ChatMessages => _chatMessages ??= new ChatMessageRepository(_context);
    public IPaymentRepository Payments => _payments ??= new PaymentRepository(_context);
    public IReservationRepository Reservations => _reservations ??= new ReservationRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
