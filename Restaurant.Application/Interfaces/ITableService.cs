using Restaurant.Application.Common;
using Restaurant.Application.DTOs.TableDTOs;
using Restaurant.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Interfaces;

public interface ITableService
{
    Task<ApiResponse<List<TableDto>>> GetAllAsync();
    Task<ApiResponse<TableDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<TableDto>> GetByQRCodeAsync(string qrCode);
    Task<ApiResponse<List<TableDto>>> GetByStatusAsync(TableStatus status);
    Task<ApiResponse<TableDto>> CreateAsync(CreateTableDto dto);
    Task<ApiResponse<TableDto>> UpdateAsync(UpdateTableDto dto);
    Task<ApiResponse<bool>> DeleteAsync(Guid id);
}
