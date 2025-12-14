using System;

namespace IARA.Business.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int AvailableQuantity { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }

    public class CreateProductDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public Guid CategoryId { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int AvailableQuantity { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}