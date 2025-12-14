using System;
using System.Collections.Generic;

namespace IARA.Business.DTOs
{
    public class SaleDto
    {
        public Guid Id { get; set; }
        public DateTime SaleDateTime { get; set; }
        public Guid StoreId { get; set; }
        public string StoreName { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public decimal TotalAmount { get; set; }
        public List<SaleItemDto> SaleItems { get; set; }
    }

    public class CreateSaleDto
    {
        public DateTime SaleDateTime { get; set; }
        public Guid StoreId { get; set; }
        public Guid EmployeeId { get; set; }
        public List<CreateSaleItemDto> SaleItems { get; set; }
    }

    public class SaleItemDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class CreateSaleItemDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}