using AutoMapper;
using Restaurant.Application.Common;
using Restaurant.Application.DTOs.ReservationDtos;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using Restaurant.Domain.Interfaces;

public class ReservationService : IReservationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ReservationService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<ReservationDto>>> GetAllAsync()
    {
        var reservations = await _unitOfWork.Reservations.GetAllAsync();
        return ApiResponse<List<ReservationDto>>.SuccessResponse(_mapper.Map<List<ReservationDto>>(reservations));
    }

    public async Task<ApiResponse<List<ReservationDto>>> GetByDateAsync(DateTime date)
    {
        var reservations = await _unitOfWork.Reservations.GetReservationsByDateAsync(date);
        return ApiResponse<List<ReservationDto>>.SuccessResponse(_mapper.Map<List<ReservationDto>>(reservations));
    }

    public async Task<ApiResponse<ReservationDto>> CreateAsync(CreateReservationDto dto)
    {
        var table = await _unitOfWork.Tables.GetByIdAsync(dto.TableId);
        if (table == null) return ApiResponse<ReservationDto>.FailResponse("Masa tapılmadı.");

        var isReserved = await _unitOfWork.Reservations.IsTableReservedAsync(dto.TableId, dto.ReservationDate);
        if (isReserved) return ApiResponse<ReservationDto>.FailResponse("Bu masa həmin tarixdə artıq rezerv edilib.");

        var reservation = _mapper.Map<Reservation>(dto);
        await _unitOfWork.Reservations.AddAsync(reservation);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<ReservationDto>.SuccessResponse(_mapper.Map<ReservationDto>(reservation), "Rezervasiya yaradıldı.");
    }

    public async Task<ApiResponse<ReservationDto>> UpdateStatusAsync(UpdateReservationStatusDto dto)
    {
        var reservation = await _unitOfWork.Reservations.GetByIdAsync(dto.Id);
        if (reservation == null) return ApiResponse<ReservationDto>.FailResponse("Tapılmadı.");

        switch (dto.Status)
        {
            case ReservationStatus.Confirmed:
                if (reservation.Status != ReservationStatus.Pending)
                    return ApiResponse<ReservationDto>.FailResponse("Yalnız gözləyən rezervasiyalar təsdiqlənə bilər.");
                reservation.Status = ReservationStatus.Confirmed;
                break;
            case ReservationStatus.Cancelled:
                if (reservation.Status == ReservationStatus.Completed)
                    return ApiResponse<ReservationDto>.FailResponse("Tamamlanmış rezervasiyalar ləğv edilə bilməz.");
                reservation.Status = ReservationStatus.Cancelled;
                break;
            case ReservationStatus.Completed:
                if (reservation.Status != ReservationStatus.Confirmed)
                    return ApiResponse<ReservationDto>.FailResponse("Yalnız təsdiqlənmiş rezervasiyalar tamamlana bilər.");
                reservation.Status = ReservationStatus.Completed;
                var table = await _unitOfWork.Tables.GetByIdAsync(reservation.TableId);
                if (table != null) table.Status = TableStatus.Occupied;
                break;
        }

        _unitOfWork.Reservations.Update(reservation);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<ReservationDto>.SuccessResponse(_mapper.Map<ReservationDto>(reservation), "Status yeniləndi.");
    }
}