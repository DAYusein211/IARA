using System;
using System.Collections.Generic;

namespace IARA.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public Guid CategoryId { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int AvailableQuantity { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        public virtual Category Category { get; set; }
        public virtual ICollection<SaleItem> SaleItems { get; set; }
    }
}