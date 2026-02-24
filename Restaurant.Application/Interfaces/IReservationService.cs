using Restaurant.Application.Common;
using Restaurant.Application.DTOs.ReservationDtos;

namespace Restaurant.Application.Interfaces;

public interface IReservationService
{
    Task<ApiResponse<List<ReservationDto>>> GetAllAsync();
    Task<ApiResponse<List<ReservationDto>>> GetByDateAsync(DateTime date);
    Task<ApiResponse<ReservationDto>> CreateAsync(CreateReservationDto dto);
    Task<ApiResponse<ReservationDto>> UpdateStatusAsync(UpdateReservationStatusDto dto);
    Task<ApiResponse<bool>> DeleteAsync(Guid id);
}
