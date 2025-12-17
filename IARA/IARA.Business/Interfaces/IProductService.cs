﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IARA.Business.DTOs;

namespace IARA.Business.Services
{
    public interface IProductService
    {
        Task<ProductDto> GetProductByIdAsync(Guid id);
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(Guid categoryId);
        Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto);
        Task<ProductDto> UpdateProductAsync(Guid id, CreateProductDto updateProductDto);
        Task DeleteProductAsync(Guid id);
        Task<bool> CheckStockAvailabilityAsync(Guid productId, int quantity);
    }
}