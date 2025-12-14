using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IARA.Business.DTOs;

namespace IARA.Business.Services
{
    public interface ISaleService
    {
        Task<SaleDto> GetSaleByIdAsync(Guid id);
        Task<IEnumerable<SaleDto>> GetAllSalesAsync();
        Task<IEnumerable<SaleDto>> GetSalesByStoreAsync(Guid storeId);
        Task<IEnumerable<SaleDto>> GetSalesByEmployeeAsync(Guid employeeId);
        Task<IEnumerable<SaleDto>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<SaleDto> CreateSaleAsync(CreateSaleDto createSaleDto);
    }
}