using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;

namespace Restaurant.Domain.Interfaces.Repositories;

public interface ITableRepository : IGenericRepository<Table>
{
    Task<Table?> GetByQRCodeAsync(string qrCode);
    Task<Table?> GetTableWithOrdersAsync(Guid tableId);
    Task<IReadOnlyList<Table>> GetTablesByStatusAsync(TableStatus status);
}







