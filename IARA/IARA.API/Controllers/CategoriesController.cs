using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using IARA.Business.DTOs;
using IARA.Business.Services;

namespace IARA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound();

            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto createCategoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await _categoryService.CreateCategoryAsync(createCategoryDto);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateCategoryDto updateCategoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await _categoryService.UpdateCategoryAsync(id, updateCategoryDto);
            if (category == null)
                return NotFound();

            return Ok(category);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            return NoContent();
        }
    }
}