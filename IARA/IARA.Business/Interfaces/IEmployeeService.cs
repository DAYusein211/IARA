﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IARA.Business.DTOs;

namespace IARA.Business.Services
{
    public interface IEmployeeService
    {
        Task<EmployeeDto> GetEmployeeByIdAsync(Guid id);
        Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync();
        Task<IEnumerable<EmployeeDto>> GetEmployeesByStoreAsync(Guid storeId);
        Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto createEmployeeDto);
        Task<EmployeeDto> UpdateEmployeeAsync(Guid id, CreateEmployeeDto updateEmployeeDto);
        Task DeleteEmployeeAsync(Guid id);
    }
}