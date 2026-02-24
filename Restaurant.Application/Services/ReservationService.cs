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
        Guid tableId;

        if (dto.TableId.HasValue && dto.TableId.Value != Guid.Empty)
        {
            // Frontend masa gonderdise yoxla
            var table = await _unitOfWork.Tables.GetByIdAsync(dto.TableId.Value);
            if (table == null) return ApiResponse<ReservationDto>.FailResponse("Masa tapilmadi.");

            var isReserved = await _unitOfWork.Reservations.IsTableReservedAsync(dto.TableId.Value, dto.ReservationDate);
            if (isReserved) return ApiResponse<ReservationDto>.FailResponse("Bu masa hemin tarixde artiq rezerv edilib.");

            tableId = dto.TableId.Value;
        }
        else
        {
            // Avtomatik uygun masa tap
            var allTables = await _unitOfWork.Tables.GetAllAsync();
            var suitableTables = allTables.Where(t => t.Capacity >= dto.GuestCount).OrderBy(t => t.Capacity).ToList();

            Guid? foundTableId = null;
            foreach (var t in suitableTables)
            {
                var isReserved = await _unitOfWork.Reservations.IsTableReservedAsync(t.Id, dto.ReservationDate);
                if (!isReserved)
                {
                    foundTableId = t.Id;
                    break;
                }
            }

            if (!foundTableId.HasValue)
                return ApiResponse<ReservationDto>.FailResponse("Hemin tarix ve qonaq sayi ucun uygun masa tapilmadi.");

            tableId = foundTableId.Value;
        }

        var reservation = _mapper.Map<Reservation>(dto);
        reservation.TableId = tableId;
        await _unitOfWork.Reservations.AddAsync(reservation);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<ReservationDto>.SuccessResponse(_mapper.Map<ReservationDto>(reservation), "Rezervasiya yaradildi.");
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
    public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
    {
        var reservation = await _unitOfWork.Reservations.GetByIdAsync(id);
        if (reservation == null) return ApiResponse<bool>.FailResponse("Tapilmadi.");

        _unitOfWork.Reservations.Delete(reservation);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "Rezervasiya silindi.");
    }
}