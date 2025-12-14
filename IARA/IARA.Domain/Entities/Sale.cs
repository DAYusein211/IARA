using System;
using System.Collections.Generic;

namespace IARA.Domain.Entities
{
    public class Sale
    {
        public Guid Id { get; set; }
        public DateTime SaleDateTime { get; set; }
        public Guid StoreId { get; set; }
        public Guid EmployeeId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        
        public virtual Store Store { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual ICollection<SaleItem> SaleItems { get; set; }
    }
}