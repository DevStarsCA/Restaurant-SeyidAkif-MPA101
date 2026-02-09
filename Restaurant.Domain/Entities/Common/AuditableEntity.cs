using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.Entities.Common;
    public abstract class AuditableEntity<T> : BaseEntity<T>
    {
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public abstract class AuditableEntity : AuditableEntity<Guid>
    {
        public AuditableEntity()
        {
            Id = Guid.NewGuid();
        }
    }

