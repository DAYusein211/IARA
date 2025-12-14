using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using IARA.Business.DTOs;
using IARA.Data;
using IARA.Domain.Entities;

namespace IARA.Business.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ProductDto> GetProductByIdAsync(Guid id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
            
            if (product == null) return null;
            return MapToDto(product);
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
            
            return products.Select(MapToDto);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(Guid categoryId)
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
            
            return products.Select(MapToDto);
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Code = createProductDto.Code,
                Name = createProductDto.Name,
                CategoryId = createProductDto.CategoryId,
                PurchasePrice = createProductDto.PurchasePrice,
                SellingPrice = createProductDto.SellingPrice,
                AvailableQuantity = createProductDto.AvailableQuantity,
                ExpirationDate = createProductDto.ExpirationDate,
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return await GetProductByIdAsync(product.Id);
        }

        public async Task<ProductDto> UpdateProductAsync(Guid id, CreateProductDto updateProductDto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return null;

            product.Code = updateProductDto.Code;
            product.Name = updateProductDto.Name;
            product.CategoryId = updateProductDto.CategoryId;
            product.PurchasePrice = updateProductDto.PurchasePrice;
            product.SellingPrice = updateProductDto.SellingPrice;
            product.AvailableQuantity = updateProductDto.AvailableQuantity;
            product.ExpirationDate = updateProductDto.ExpirationDate;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return await GetProductByIdAsync(id);
        }

        public async Task DeleteProductAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> CheckStockAvailabilityAsync(Guid productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            return product != null && product.AvailableQuantity >= quantity;
        }

        private ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Code = product.Code,
                Name = product.Name,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                PurchasePrice = product.PurchasePrice,
                SellingPrice = product.SellingPrice,
                AvailableQuantity = product.AvailableQuantity,
                ExpirationDate = product.ExpirationDate
            };
        }
    }
}