namespace Restaurant.Domain.Enums;

public enum PaymentStatus
    {
        Pending,        // Gözləyir
        Completed,      // Ödənilib
        Failed,         // Uğursuz
        Refunded        // Geri qaytarılıb
    }
