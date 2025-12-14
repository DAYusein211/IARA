using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using IARA.Business.DTOs;
using IARA.Business.Services;

namespace IARA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoresController : ControllerBase
    {
        private readonly IStoreService _storeService;

        public StoresController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var stores = await _storeService.GetAllStoresAsync();
            return Ok(stores);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var store = await _storeService.GetStoreByIdAsync(id);
            if (store == null)
                return NotFound();

            return Ok(store);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStoreDto createStoreDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var store = await _storeService.CreateStoreAsync(createStoreDto);
            return CreatedAtAction(nameof(GetById), new { id = store.Id }, store);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateStoreDto updateStoreDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var store = await _storeService.UpdateStoreAsync(id, updateStoreDto);
            if (store == null)
                return NotFound();

            return Ok(store);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _storeService.DeleteStoreAsync(id);
            return NoContent();
        }
    }
}