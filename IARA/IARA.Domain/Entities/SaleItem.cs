using System;

namespace IARA.Domain.Entities
{
    public class SaleItem
    {
        public Guid Id { get; set; }
        public Guid SaleId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
        
        public virtual Sale Sale { get; set; }
        public virtual Product Product { get; set; }
    }
}