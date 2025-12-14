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
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _context;

        public EmployeeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EmployeeDto> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _context.Employees
                .Include(e => e.Store)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null) return null;
            return MapToDto(employee);
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
        {
            var employees = await _context.Employees
                .Include(e => e.Store)
                .ToListAsync();

            return employees.Select(MapToDto);
        }

        public async Task<IEnumerable<EmployeeDto>> GetEmployeesByStoreAsync(Guid storeId)
        {
            var employees = await _context.Employees
                .Include(e => e.Store)
                .Where(e => e.StoreId == storeId)
                .ToListAsync();

            return employees.Select(MapToDto);
        }

        public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto createEmployeeDto)
        {
            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = createEmployeeDto.FirstName,
                LastName = createEmployeeDto.LastName,
                Position = createEmployeeDto.Position,
                MonthlySalary = createEmployeeDto.MonthlySalary,
                EmploymentStartDate = createEmployeeDto.EmploymentStartDate,
                StoreId = createEmployeeDto.StoreId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return await GetEmployeeByIdAsync(employee.Id);
        }

        public async Task<EmployeeDto> UpdateEmployeeAsync(Guid id, CreateEmployeeDto updateEmployeeDto)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return null;

            employee.FirstName = updateEmployeeDto.FirstName;
            employee.LastName = updateEmployeeDto.LastName;
            employee.Position = updateEmployeeDto.Position;
            employee.MonthlySalary = updateEmployeeDto.MonthlySalary;
            employee.EmploymentStartDate = updateEmployeeDto.EmploymentStartDate;
            employee.StoreId = updateEmployeeDto.StoreId;
            employee.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return await GetEmployeeByIdAsync(id);
        }

        public async Task DeleteEmployeeAsync(Guid id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
        }

        private EmployeeDto MapToDto(Employee employee)
        {
            return new EmployeeDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Position = employee.Position,
                MonthlySalary = employee.MonthlySalary,
                EmploymentStartDate = employee.EmploymentStartDate,
                StoreId = employee.StoreId,
                StoreName = employee.Store?.Name
            };
        }
    }
}