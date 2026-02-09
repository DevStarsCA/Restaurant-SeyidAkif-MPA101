using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.Enums;
    public enum OrderStatus
    {
        Pending,        // Sifariş verildi, gözləyir
        Preparing,      // Mətbəxdə hazırlanır
        Ready,          // Hazırdır, ofisiant gətirməlidir
        Delivered,      // Masaya çatdırılıb
        Cancelled       // Ləğv edilib
    }
