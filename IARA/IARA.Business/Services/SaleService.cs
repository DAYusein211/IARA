﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using IARA.Business.DTOs;
using IARA.Data;
using IARA.Domain.Entities;

namespace IARA.Business.Services
{
    public class SaleService : ISaleService
    {
        private readonly ApplicationDbContext _context;

        public SaleService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SaleDto> GetSaleByIdAsync(Guid id)
        {
            var sale = await _context.Sales
                .Include(s => s.Store)
                .Include(s => s.Employee)
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.Product)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale == null) return null;
            return MapToDto(sale);
        }

        public async Task<IEnumerable<SaleDto>> GetAllSalesAsync()
        {
            var sales = await _context.Sales
                .Include(s => s.Store)
                .Include(s => s.Employee)
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.Product)
                .OrderByDescending(s => s.SaleDateTime)
                .ToListAsync();

            return sales.Select(MapToDto);
        }

        public async Task<IEnumerable<SaleDto>> GetSalesByStoreAsync(Guid storeId)
        {
            var sales = await _context.Sales
                .Include(s => s.Store)
                .Include(s => s.Employee)
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.Product)
                .Where(s => s.StoreId == storeId)
                .OrderByDescending(s => s.SaleDateTime)
                .ToListAsync();

            return sales.Select(MapToDto);
        }

        public async Task<IEnumerable<SaleDto>> GetSalesByEmployeeAsync(Guid employeeId)
        {
            var sales = await _context.Sales
                .Include(s => s.Store)
                .Include(s => s.Employee)
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.Product)
                .Where(s => s.EmployeeId == employeeId)
                .OrderByDescending(s => s.SaleDateTime)
                .ToListAsync();

            return sales.Select(MapToDto);
        }

        public async Task<IEnumerable<SaleDto>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var sales = await _context.Sales
                .Include(s => s.Store)
                .Include(s => s.Employee)
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.Product)
                .Where(s => s.SaleDateTime >= startDate && s.SaleDateTime <= endDate)
                .OrderByDescending(s => s.SaleDateTime)
                .ToListAsync();

            return sales.Select(MapToDto);
        }

        public async Task<SaleDto> CreateSaleAsync(CreateSaleDto createSaleDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                // Validate that store exists
                var storeExists = await _context.Stores.AnyAsync(s => s.Id == createSaleDto.StoreId);
                if (!storeExists)
                    throw new Exception($"Store with ID {createSaleDto.StoreId} does not exist.");

                // Validate that employee exists
                var employeeExists = await _context.Employees.AnyAsync(e => e.Id == createSaleDto.EmployeeId);
                if (!employeeExists)
                    throw new Exception($"Employee with ID {createSaleDto.EmployeeId} does not exist.");

                // Check for duplicate sale (same datetime, store, and employee within same minute)
                var sameDateTimeStart = createSaleDto.SaleDateTime.AddSeconds(-createSaleDto.SaleDateTime.Second).AddMilliseconds(-createSaleDto.SaleDateTime.Millisecond);
                var sameDateTimeEnd = sameDateTimeStart.AddSeconds(59).AddMilliseconds(999);

                var duplicateSale = await _context.Sales.AnyAsync(s =>
                    s.SaleDateTime >= sameDateTimeStart &&
                    s.SaleDateTime <= sameDateTimeEnd &&
                    s.StoreId == createSaleDto.StoreId &&
                    s.EmployeeId == createSaleDto.EmployeeId);

                if (duplicateSale)
                    throw new Exception("A sale with this datetime, store, and employee combination already exists.");

                var sale = new Sale
                {
                    Id = Guid.NewGuid(),
                    SaleDateTime = createSaleDto.SaleDateTime,
                    StoreId = createSaleDto.StoreId,
                    EmployeeId = createSaleDto.EmployeeId,
                    CreatedAt = DateTime.UtcNow,
                    SaleItems = new List<SaleItem>()
                };

                decimal totalAmount = 0;

                foreach (var item in createSaleDto.SaleItems)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product == null)
                        throw new Exception($"Product with ID {item.ProductId} does not exist.");

                    if (product.AvailableQuantity < item.Quantity)
                        throw new Exception($"Insufficient stock for product {product.Name}. Available: {product.AvailableQuantity}, Requested: {item.Quantity}");

                    var subtotal = product.SellingPrice * item.Quantity;
                    totalAmount += subtotal;

                    var saleItem = new SaleItem
                    {
                        Id = Guid.NewGuid(),
                        SaleId = sale.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = product.SellingPrice,
                        Subtotal = subtotal
                    };

                    sale.SaleItems.Add(saleItem);
                    product.AvailableQuantity -= item.Quantity;
                }

                sale.TotalAmount = totalAmount;

                _context.Sales.Add(sale);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await GetSaleByIdAsync(sale.Id);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        private SaleDto MapToDto(Sale sale)
        {
            return new SaleDto
            {
                Id = sale.Id,
                SaleDateTime = sale.SaleDateTime,
                StoreId = sale.StoreId,
                StoreName = sale.Store?.Name,
                EmployeeId = sale.EmployeeId,
                EmployeeName = $"{sale.Employee?.FirstName} {sale.Employee?.LastName}",
                TotalAmount = sale.TotalAmount,
                SaleItems = sale.SaleItems?.Select(si => new SaleItemDto
                {
                    Id = si.Id,
                    ProductId = si.ProductId,
                    ProductName = si.Product?.Name,
                    Quantity = si.Quantity,
                    UnitPrice = si.UnitPrice,
                    Subtotal = si.Subtotal
                }).ToList()
            };
        }
    }
}