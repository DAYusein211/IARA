using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using IARA.Business.DTOs;
using IARA.Business.Services;

namespace IARA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        [HttpGet("store/{storeId}")]
        public async Task<IActionResult> GetByStore(Guid storeId)
        {
            var employees = await _employeeService.GetEmployeesByStoreAsync(storeId);
            return Ok(employees);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeDto createEmployeeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var employee = await _employeeService.CreateEmployeeAsync(createEmployeeDto);
            return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateEmployeeDto updateEmployeeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var employee = await _employeeService.UpdateEmployeeAsync(id, updateEmployeeDto);
            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _employeeService.DeleteEmployeeAsync(id);
            return NoContent();
        }
    }
}