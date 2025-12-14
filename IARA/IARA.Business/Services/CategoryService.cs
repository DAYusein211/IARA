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
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return null;

            return MapToDto(category);
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories.ToListAsync();
            return categories.Select(MapToDto);
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = createCategoryDto.Name,
                Description = createCategoryDto.Description,
                CreatedAt = DateTime.UtcNow
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return MapToDto(category);
        }

        public async Task<CategoryDto> UpdateCategoryAsync(Guid id, CreateCategoryDto updateCategoryDto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return null;

            category.Name = updateCategoryDto.Name;
            category.Description = updateCategoryDto.Description;
            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return MapToDto(category);
        }

        public async Task DeleteCategoryAsync(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }

        private CategoryDto MapToDto(Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }
    }
}