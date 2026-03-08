using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.Entities.Common
{
    public abstract class BaseEntity<T>
    {
        public T Id { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
    }

    public abstract class BaseEntity : BaseEntity<Guid>
    {
        public BaseEntity()
        {
            Id = Guid.NewGuid();
        }
    }
}