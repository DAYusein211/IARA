using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IARA.Business.DTOs;

namespace IARA.Business.Services
{
    public interface ICategoryService
    {
        Task<CategoryDto> GetCategoryByIdAsync(Guid id);
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto);
        Task<CategoryDto> UpdateCategoryAsync(Guid id, CreateCategoryDto updateCategoryDto);
        Task DeleteCategoryAsync(Guid id);
    }
}